import logging  # Importation de la bibliothèque logging pour la gestion des logs
import os  # Importation de la bibliothèque os pour les opérations système

class LogGen:
    """
    Classe LogGen pour générer des logs dans un fichier spécifié.
    """

    def __init__(self, log_file_path, log_level=logging.INFO):
        """
        Initialisation de la classe LogGen.

        Args:
            log_file_path (str): Le chemin du fichier de log.
            log_level (int, optional): Le niveau de log. Par défaut, c'est logging.INFO.
        """
        self.log_file_path = log_file_path  # Chemin du fichier de log
        self.logger = logging.getLogger(__name__)  # Création d'un logger
        self.logger.setLevel(log_level)  # Définition du niveau de log

        # Création d'un formatter pour définir le format des messages de log
        formatter = logging.Formatter('%(asctime)s - %(name)s - %(levelname)s - %(message)s')
        
        # Création d'un handler pour écrire les logs dans le fichier spécifié
        handler = logging.FileHandler(self.log_file_path)
        handler.setFormatter(formatter)  # Application du formatter au handler

        # Ajout du handler au logger
        self.logger.addHandler(handler)

    def log_info(self, message):
        """
        Méthode pour enregistrer un message de niveau INFO.

        Args:
            message (str): Le message à enregistrer.
        """
        self.logger.info(message)  # Enregistrement d'un message de niveau INFO

    def log_warning(self, message):
        """
        Méthode pour enregistrer un message de niveau WARNING.

        Args:
            message (str): Le message à enregistrer.
        """
        self.logger.warning(message)  # Enregistrement d'un message de niveau WARNING

    def log_error(self, message):
        """
        Méthode pour enregistrer un message de niveau ERROR.

        Args:
            message (str): Le message à enregistrer.
        """
        self.logger.error(message)  # Enregistrement d'un message de niveau ERROR

    def log_critical(self, message):
        """
        Méthode pour enregistrer un message de niveau CRITICAL.

        Args:
            message (str): Le message à enregistrer.
        """
        self.logger.critical(message)  # Enregistrement d'un message de niveau CRITICAL
