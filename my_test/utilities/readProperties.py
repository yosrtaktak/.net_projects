import configparser  # Importation de la bibliothèque configparser pour gérer les fichiers de configuration

# Création d'un objet RawConfigParser pour lire les fichiers de configuration
config = configparser.RawConfigParser()

# Lecture du fichier de configuration situé dans le répertoire ./Configurations
config.read('./Configurations/config.ini')

class ReadConfig():
    """
    Classe ReadConfig pour lire les paramètres de configuration depuis le fichier config.ini.
    """

    @staticmethod
    def getApplicationURL():
        """
        Méthode statique pour obtenir l'URL de l'application depuis la section [common info].
        
        Returns:
            str: L'URL de l'application.
        """
        url = config.get('common info', 'baseURL')  # Lecture de la valeur 'baseURL' de la section 'common info'
        return url

    @staticmethod
    def getUseremail():
        """
        Méthode statique pour obtenir l'email de l'utilisateur depuis la section [common info].
        
        Returns:
            str: L'email de l'utilisateur.
        """
        username = config.get('common info', 'useremail')  # Lecture de la valeur 'useremail' de la section 'common info'
        return username

    @staticmethod
    def getPassword():
        """
        Méthode statique pour obtenir le mot de passe de l'utilisateur depuis la section [common info].
        
        Returns:
            str: Le mot de passe de l'utilisateur.
        """
        password = config.get('common info', 'password')  # Lecture de la valeur 'password' de la section 'common info'
        return password
