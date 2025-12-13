import configparser
import os

# Création d'un objet RawConfigParser pour lire les fichiers de configuration
config = configparser.RawConfigParser()

# Lecture du fichier de configuration
config_path = os.path.join(os.path.dirname(__file__), '..', 'Configurations', 'config.ini')
config.read(config_path)

class ReadConfig:
    """
    Classe ReadConfig pour lire les paramètres de configuration depuis le fichier config.ini.
    """

    @staticmethod
    def getBaseURL():
        """
        Obtenir l'URL de base du frontend.
        
        Returns:
            str: L'URL du frontend.
        """
        return config.get('common info', 'baseURL')

    @staticmethod
    def getApiURL():
        """
        Obtenir l'URL de l'API backend.
        
        Returns:
            str: L'URL de l'API backend.
        """
        return config.get('common info', 'apiURL')

    @staticmethod
    def getAdminUsername():
        """
        Obtenir le nom d'utilisateur admin.
        
        Returns:
            str: Le nom d'utilisateur admin.
        """
        return config.get('credentials', 'admin_username')

    @staticmethod
    def getAdminPassword():
        """
        Obtenir le mot de passe admin.
        
        Returns:
            str: Le mot de passe admin.
        """
        return config.get('credentials', 'admin_password')

    @staticmethod
    def getEmployeeUsername():
        """
        Obtenir le nom d'utilisateur employee.
        
        Returns:
            str: Le nom d'utilisateur employee.
        """
        return config.get('credentials', 'employee_username')

    @staticmethod
    def getEmployeePassword():
        """
        Obtenir le mot de passe employee.
        
        Returns:
            str: Le mot de passe employee.
        """
        return config.get('credentials', 'employee_password')

    @staticmethod
    def getCustomerUsername():
        """
        Obtenir le nom d'utilisateur customer.
        
        Returns:
            str: Le nom d'utilisateur customer.
        """
        return config.get('credentials', 'customer_username')

    @staticmethod
    def getCustomerPassword():
        """
        Obtenir le mot de passe customer.
        
        Returns:
            str: Le mot de passe customer.
        """
        return config.get('credentials', 'customer_password')

    @staticmethod
    def getBrowser():
        """
        Obtenir le type de navigateur.
        
        Returns:
            str: Le type de navigateur.
        """
        return config.get('selenium', 'browser')

    @staticmethod
    def getImplicitWait():
        """
        Obtenir le temps d'attente implicite.
        
        Returns:
            int: Le temps d'attente en secondes.
        """
        return int(config.get('selenium', 'implicit_wait'))

    @staticmethod
    def getScreenshotsDir():
        """
        Obtenir le répertoire des screenshots.
        
        Returns:
            str: Le chemin du répertoire des screenshots.
        """
        return config.get('paths', 'screenshots_dir')

    @staticmethod
    def getLogsDir():
        """
        Obtenir le répertoire des logs.
        
        Returns:
            str: Le chemin du répertoire des logs.
        """
        return config.get('paths', 'logs_dir')
