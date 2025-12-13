import pytest
from time import sleep
from pages.login_page import LoginPage
from utilities.readProperties import ReadConfig
from utilities.customLogger import LogGen


class Test_001_Login:
    """
    Classe de tests pour la fonctionnalité de connexion.
    Suit le pattern de my_test avec ReadConfig et CustomLogger.
    """
    
    # Lecture des configurations depuis le fichier de configuration
    baseURL = ReadConfig.getBaseURL()
    username = ReadConfig.getAdminUsername()
    password = ReadConfig.getAdminPassword()
    
    # Initialisation du logger
    logger = LogGen("./Logs/automation.log")
    
    def test_TC023_homePageTitle(self, setup):
        """Test du titre de la page d'accueil"""
        self.logger.log_info("*************** Test_001_Login ***************")
        self.logger.log_info("**** Started Home page title test ****")
        
        self.driver = setup
        self.logger.log_info(f"**** Opening URL: {self.baseURL} ****")
        self.driver.get(self.baseURL)
        sleep(2)
        
        actualTitle = self.driver.title
        self.logger.log_info(f"**** Page title: {actualTitle} ****")
        
        if "Car Rental" in actualTitle or "Blazor" in actualTitle:
            self.logger.log_info("**** Home page title test passed ****")
            self.driver.close()
            assert True
        else:
            self.logger.log_error("**** Home page title test failed ****")
            self.driver.save_screenshot("./Screenshots/test_homePageTitle.png")
            self.driver.close()
            assert False
    
    @pytest.mark.ui
    @pytest.mark.auth
    def test_TC023_login_valid_credentials(self, setup):
        """Test de connexion avec des identifiants valides"""
        self.logger.log_info("**** Started Login Test ****")
        
        self.driver = setup
        self.driver.get(f"{self.baseURL}/login")
        sleep(3)  # Attendre que Blazor charge
        
        # Création de l'instance de LoginPage
        self.loginPage = LoginPage(self.driver)
        
        self.logger.log_info(f"**** Setting username: {self.username} ****")
        self.loginPage.setUserName(self.username)
        sleep(1)
        
        self.logger.log_info("**** Setting password ****")
        self.loginPage.setPassword(self.password)
        sleep(1)
        
        self.logger.log_info("**** Clicking login button ****")
        self.loginPage.clickLogin()
        sleep(3)
        
        # Vérifier si la connexion a réussi
        if self.loginPage.isLoginSuccessful():
            self.logger.log_info("**** Login test passed ****")
            self.driver.close()
            assert True
        else:
            self.logger.log_error("**** Login test failed ****")
            self.driver.save_screenshot("./Screenshots/test_login.png")
            self.driver.close()
            assert False
    
    @pytest.mark.ui
    @pytest.mark.auth
    def test_TC024_login_invalid_password(self, setup):
        """Test de connexion avec mot de passe incorrect"""
        self.logger.log_info("**** Started Invalid Login Test ****")
        
        self.driver = setup
        self.driver.get(f"{self.baseURL}/login")
        sleep(3)  # Attendre que Blazor charge
        
        # Création de l'instance de LoginPage
        self.loginPage = LoginPage(self.driver)
        
        self.logger.log_info(f"**** Setting username: {self.username} ****")
        self.loginPage.setUserName(self.username)
        sleep(1)
        
        self.logger.log_info("**** Setting wrong password ****")
        self.loginPage.setPassword("WrongPassword123")
        sleep(1)
        
        self.logger.log_info("**** Clicking login button ****")
        self.loginPage.clickLogin()
        sleep(3)
        
        # Vérifier qu'un message d'erreur est affiché ou que l'utilisateur reste sur /login
        current_url = self.loginPage.getCurrentURL()
        if "/login" in current_url.lower() or self.loginPage.isErrorDisplayed():
            self.logger.log_info("**** Invalid login test passed - Error displayed ****")
            self.driver.close()
            assert True
        else:
            self.logger.log_error("**** Invalid login test failed - No error displayed ****")
            self.driver.save_screenshot("./Screenshots/test_login_invalid.png")
            self.driver.close()
            assert False
