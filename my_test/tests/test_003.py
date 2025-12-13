import pytest  
from selenium import webdriver 
from selenium.webdriver.common.by import By
from pages.LoginPage import LoginPage  
from time import sleep
from utilities.readProperties import ReadConfig  
from utilities.newCustomLogger import LogGen 


class Test_003_Login:
        # Lecture des configurations depuis le fichier de configuration
    baseUrl = ReadConfig.getApplicationURL()
    username = ReadConfig.getUseremail()
    password = ReadConfig.getPassword()

    # Initialisation du logger pour enregistrer les logs des tests
    logger = LogGen("./Logs/automation.log")
    #logger = LogGen.loggen()  # Initialisation alternative commentée du logger

    def test_homePageTitle(self, setup):
        self.logger.log_info("*************** Test_003_Login *****************")
        self.logger.log_info("****Started Home page title test ****")
        self.driver = setup
        self.logger.log_info("****Opening URL****")
        self.driver.get(self.baseUrl)
        act_title = self.driver.title

        if act_title == "Swag Labs":
            self.logger.log_info("**** Home page title test passed ****")
            self.driver.close()
            assert True
        else:
            self.logger.log_error("**** Home page title test failed****")
            self.driver.save_screenshot("./tests/Screenshots/" + "test_homePageTitle.png")
            self.driver.close()
            assert False

    def test_login(self, setup):
        self.logger.log_info("****Started Login Test****")
        self.driver = setup
        self.driver.get(self.baseUrl)
                # Création d'une instance de LoginPage pour interagir avec les éléments de la page de connexion
        self.lp = LoginPage(self.driver)
        self.lp.setUserName(self.username)
        self.lp.setPassword(self.password)
        self.lp.clickLogin()
        text = self.driver.find_element(By.CLASS_NAME, "title").text
        if text == "Products":
            self.logger.log_info("**** Test Authentication passed ****")
            self.driver.close()
            assert True
        else:
            self.logger.log_error("**** Test Authentication failed****")
            self.driver.save_screenshot("./tests/screenshots/" + "test_login.png")
            self.driver.close()
            assert False
    def test_logout(self, setup):
        self.logger.log_info("*************** Test_003_Logout *****************")
        self.driver = setup
        self.driver.get(self.baseUrl)
                # Création d'une instance de LoginPage pour interagir avec les éléments de la page de connexion
        self.lp = LoginPage(self.driver)
        # Création de l'instance de LoginPage
        self.loginPage = LoginPage(self.driver)
        # Utilisation des méthodes pour interagir avec la page
        self.loginPage.setUserName(self.username)
        self.loginPage.setPassword(self.password)
        self.loginPage.clickLogin()
        sleep(3)
        self.loginPage.clickLogout()
        element=self.driver.current_url
        
        if element == "https://www.saucedemo.com/":
            self.logger.log_info("**** Test Authentication Non passing case  passed ****")
            self.driver.close()
            assert True
        else:
            self.logger.log_error("**** Test Authentication Non passing case  failed****")
            self.driver.save_screenshot("./tests/screenshots/" + "test_login.png")
            self.driver.close()
            assert False
        sleep(2)
