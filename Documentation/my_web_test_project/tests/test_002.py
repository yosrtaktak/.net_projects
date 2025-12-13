from selenium import webdriver
from time import sleep
from pages.LoginPage import LoginPage
from selenium.webdriver.common.by import By
import pytest
import os
class Test_002_Login:
    url = "https://www.saucedemo.com/"
    username = "standard_user"
    password = "secret_sauce"
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
        self.loginPage = LoginPage(self.driver)
        self.loginPage.setUserName(self.username)
        self.loginPage.setPassword(self.password)
        self.loginPage.clickLogin()
        url = self.driver.current_url
        if url == "https://www.saucedemo.com/inventory.html":
            self.driver.close()
            assert True
        else:
            self.driver.save_screenshot("./tests/screenshots/" + "test_login.png")
            self.driver.close()
            assert False
           
    def test_logout(self, setup):
        self.driver = setup
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



            