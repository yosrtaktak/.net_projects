import logging
import os
from datetime import datetime

class LogGen:
    """
    Classe pour générer des logs personnalisés pour les tests d'automatisation.
    """
    
    def __init__(self, log_file=None):
        """
        Initialise le logger avec un fichier de log spécifique.
        
        Args:
            log_file: Chemin du fichier de log. Si None, utilise un nom par défaut.
        """
        if log_file is None:
            # Créer le répertoire Logs s'il n'existe pas
            logs_dir = './Logs'
            os.makedirs(logs_dir, exist_ok=True)
            log_file = os.path.join(logs_dir, 'automation.log')
        else:
            # S'assurer que le répertoire existe
            log_dir = os.path.dirname(log_file)
            if log_dir:
                os.makedirs(log_dir, exist_ok=True)
        
        # Configuration du logger
        self.logger = logging.getLogger(__name__)
        self.logger.setLevel(logging.INFO)
        
        # Éviter les handlers dupliqués
        if not self.logger.handlers:
            # Handler pour le fichier
            file_handler = logging.FileHandler(log_file, mode='a', encoding='utf-8')
            file_handler.setLevel(logging.INFO)
            
            # Format des logs
            formatter = logging.Formatter(
                '%(asctime)s - %(levelname)s - %(message)s',
                datefmt='%m/%d/%Y %I:%M:%S %p'
            )
            file_handler.setFormatter(formatter)
            
            # Ajouter le handler au logger
            self.logger.addHandler(file_handler)
    
    def log_info(self, message):
        """
        Enregistre un message de niveau INFO.
        
        Args:
            message: Le message à enregistrer.
        """
        self.logger.info(message)
    
    def log_warning(self, message):
        """
        Enregistre un message de niveau WARNING.
        
        Args:
            message: Le message à enregistrer.
        """
        self.logger.warning(message)
    
    def log_error(self, message):
        """
        Enregistre un message de niveau ERROR.
        
        Args:
            message: Le message à enregistrer.
        """
        self.logger.error(message)
    
    def log_debug(self, message):
        """
        Enregistre un message de niveau DEBUG.
        
        Args:
            message: Le message à enregistrer.
        """
        self.logger.debug(message)

    @staticmethod
    def loggen():
        """
        Méthode statique pour créer un logger simple (compatibilité avec l'ancien code).
        
        Returns:
            logging.Logger: Une instance de logger configurée.
        """
        logs_dir = './Logs'
        os.makedirs(logs_dir, exist_ok=True)
        log_file = os.path.join(logs_dir, 'automation.log')
        
        logging.basicConfig(
            filename=log_file,
            format='%(asctime)s: %(levelname)s: %(message)s',
            datefmt='%m/%d/%Y %I:%M:%S %p',
            level=logging.INFO
        )
        logger = logging.getLogger()
        logger.setLevel(logging.INFO)
        return logger
