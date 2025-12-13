from selenium import webdriver
from selenium.webdriver.common.by import By
from pages.LoginPage import LoginPage
from time import sleep
import pytest
class Test_001_Login:
    url = "https://www.saucedemo.com/"
    username = "standard_user"
    password = "secret_sauce"
    def test_homePageTitle(self):
        self.driver = webdriver.Chrome()
        self.driver.get(self.url)
        sleep(3)
        actualTitle = self.driver.title
        assert actualTitle == "Swag Labs"
        self.driver.close()
        print("Test for home page passed")
    def test_login(self):
        self.driver = webdriver.Chrome()
        self.driver.get(self.url)
        # Création de l'instance de LoginPage
        self.loginPage = LoginPage(self.driver)
        sleep(2)
        # Utilisation des méthodes pour interagir avec la page
        self.loginPage.setUserName(self.username)
        sleep(2)
        self.loginPage.setPassword(self.password)
        sleep(2)
        self.loginPage.clickLogin()
        sleep(2)
        text = self.driver.find_element(By.CLASS_NAME, "title").text
# Check if login was successful 
        assert "products" in text.lower()
        
    def test_logout(self):
        self.driver = webdriver.Chrome()
        self.driver.get(self.url)
        # Création de l'instance de LoginPage
        self.loginPage = LoginPage(self.driver)
        # Utilisation des méthodes pour interagir avec la page
        self.loginPage.setUserName(self.username)
        self.loginPage.setPassword(self.password)
        self.loginPage.clickLogin()
        sleep(3)
        self.loginPage.clickLogout()
        element=self.driver.find_element(By.ID, "login-button")
        assert element is not None
        sleep(2)
       
           




            