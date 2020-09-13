using Newtonsoft.Json;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Extensions;
using ProjetBase.Businnes.Models;
using ProjetBase.Businnes.Repositories.Parametrage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Outils.Messagerie
{
    public class Messagerie
    {
        ProjetBaseContext DbContext;

        private readonly IParametrageRepository parametrageRepository;

        public Messagerie(ProjetBaseContext _context)
        {
            DbContext = _context;
            parametrageRepository = new ParametrageRepository(_context);
        }

        public MessagerResponseModel SendEmail(InoMessagerie.Models.SendMailParamsModel MailParams)
        {


            //initialser
            var ServerInfos = getConfigurationMessagerie();

            if (ServerInfos == null)
            {
                return new MessagerResponseModel
                {
                    statut = 300,
                    message = "your mailing server has no configuration"
                };
            }

            InoMessagerie.Messagerie msg = new InoMessagerie.Messagerie(ServerInfos);

            try
            {
                bool result = msg.SendMail(MailParams);
                if (result)
                {
                    return new MessagerResponseModel
                    {
                        statut = 200,
                        message = "email send sussccefly"
                    };
                }
                else
                {
                    return new MessagerResponseModel
                    {
                        statut = 300,
                        message = "error"
                    };
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "302")
                {
                    return new MessagerResponseModel
                    {
                        statut = 301,
                        message = "invalid arguments"
                    };
                }
                if (ex.Message == "301")
                {
                    return new MessagerResponseModel
                    {
                        statut = 302,
                        message = "error în server connection"
                    };
                }
                return new MessagerResponseModel
                {
                    statut = 300,
                    message = "error"
                };
            }
        }

        public InoMessagerie.Models.ServerConfigurationModel getConfigurationMessagerie()
        {
            var currentUser = EntityExtensions.GetCurrentUser(DbContext);

            //recupéer la configuration du messagerie
            var param = parametrageRepository.GetParametrageMessagerie();
            ConfigMessagerieModel configMessagerie = JsonConvert.DeserializeObject<ConfigMessagerieModel>(param.Contenu);

            //vérifer si la config du serveur est exists
            bool condition = (configMessagerie == null || configMessagerie.server.Equals("") || configMessagerie.port.Equals("") || configMessagerie.password.Equals("") || configMessagerie.username.Equals("") || configMessagerie.ssl.Equals(""));

            if (condition)
            {
                return null;
            }

            return new InoMessagerie.Models.ServerConfigurationModel
            {
                server = configMessagerie.server,
                port = configMessagerie.port,
                username = configMessagerie.username,
                password = configMessagerie.password,
                useSsl = configMessagerie.ssl,
            };
        }
    }
}
