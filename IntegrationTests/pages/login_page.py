# -*- coding: utf-8 -*-
"""
Page Object Model pour la page de connexion
Pattern: Page Object Model (POM)
"""
from selenium.webdriver.common.by import By
from time import sleep


class LoginPage:
    """
    Page Object pour la page de connexion.
    Encapsule toutes les interactions avec la page /login.
    """
    
    # Localisateurs pour MudBlazor components
    textbox_username_css = 'input[type="text"]'
    textbox_password_css = 'input[type="password"]'
    button_login_css = 'button[type="submit"]'
    error_message_css = '.mud-alert-error, .mud-snackbar-content-message'
    
    def __init__(self, driver):
        """
        Initialiser la LoginPage.
        
        Args:
            driver: Instance du WebDriver Selenium
        """
        self.driver = driver
    
    def setUserName(self, username):
        """
        Entrer le nom d'utilisateur dans le champ approprié.
        
        Args:
            username: Nom d'utilisateur à saisir
        """
        username_field = self.driver.find_element(By.CSS_SELECTOR, self.textbox_username_css)
        username_field.clear()
        username_field.send_keys(username)
    
    def setPassword(self, password):
        """
        Entrer le mot de passe.
        
        Args:
            password: Mot de passe à saisir
        """
        password_field = self.driver.find_element(By.CSS_SELECTOR, self.textbox_password_css)
        password_field.clear()
        password_field.send_keys(password)
    
    def clickLogin(self):
        """Cliquer sur le bouton de connexion"""
        login_btn = self.driver.find_element(By.CSS_SELECTOR, self.button_login_css)
        login_btn.click()
    
    def isErrorDisplayed(self):
        """
        Vérifier si un message d'erreur est affiché.
        
        Returns:
            bool: True si un message d'erreur est visible
        """
        try:
            error = self.driver.find_element(By.CSS_SELECTOR, self.error_message_css)
            return error.is_displayed()
        except:
            return False
    
    def getCurrentURL(self):
        """Retourner l'URL actuelle"""
        return self.driver.current_url
    
    def isLoginSuccessful(self):
        """
        Vérifier si la connexion a réussi (redirection).
        
        Returns:
            bool: True si redirigé vers dashboard/home ou admin
        """
        sleep(2)  # Attendre la redirection
        current_url = self.driver.current_url.lower()
        return '/login' not in current_url
