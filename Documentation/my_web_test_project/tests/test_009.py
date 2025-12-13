from selenium import webdriver
from selenium.webdriver.common.by import By
#from selenium.webdriver.chrome.service import Service
from pages.LoginPage import LoginPage
from time import sleep
import os
import pytest
class Test_009_Login:
    url = "https://www.saucedemo.com/"
    username = "standard_user"
    password = "secret_sauceees"
    def test_homePageTitle(self, setup):
        self.driver = setup
        self.driver.get(self.url)
        actualTitle = self.driver.title
        if actualTitle == "Swag Labs":
            self.driver.close()
            assert True
        else:
            self.driver.save_screenshot("./tests/screenshots/test_homePageTitle.png")
            self.driver.close()
            assert False
        
    def test_login(self, setup):
        self.driver = setup
        self.driver.get(self.url)
        # Création de l'instance de LoginPage
        self.loginPage = LoginPage(self.driver)
        # Utilisation des méthodes pour interagir avec la page
        self.loginPage.setUserName(self.username)
        self.loginPage.setPassword(self.password)
        self.loginPage.clickLogin()
        url=self.driver.current_url
# Check if login was successful 
      
        if url == "https://www.saucedemo.com/inventory.html":
            self.driver.close()
            assert True
        else:
            self.driver.save_screenshot("./tests/screenshots/" + "test_login.png")
            self.driver.close()
            assert False
    