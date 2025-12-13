# -*- coding: utf-8 -*-
"""
Page Object Model pour la page de connexion
Pattern: Page Object Model (POM)
"""
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from selenium.common.exceptions import TimeoutException


class LoginPage:
    """
    Page Object pour la page de connexion
    Encapsule toutes les interactions avec la page /login
    """
    
    # Localisateurs pour MudBlazor components
    # MudTextField génère des inputs avec des classes spécifiques
    USERNAME_INPUT = (By.CSS_SELECTOR, 'input[aria-label="Username"], input[type="text"]')
    PASSWORD_INPUT = (By.CSS_SELECTOR, 'input[type="password"]')
    LOGIN_BUTTON = (By.CSS_SELECTOR, 'button[type="submit"]')
    ERROR_MESSAGE = (By.CSS_SELECTOR, '.mud-alert-error, .mud-snackbar-content-message')
    SUCCESS_MESSAGE = (By.CSS_SELECTOR, '.mud-alert-success, .mud-snackbar-content-message')
    
    def __init__(self, driver):
        """
        Initialiser la LoginPage
        
        Args:
            driver: Instance du WebDriver Selenium
        """
        self.driver = driver
        self.wait = WebDriverWait(driver, 15)  # Augmenté à 15 secondes pour Blazor
    
    def navigate_to(self, base_url):
        """Naviguer vers la page de connexion"""
        self.driver.get(f'{base_url}/login')
        # Attendre que la page Blazor soit chargée
        self.wait.until(lambda d: d.execute_script('return document.readyState') == 'complete')
    
    def enter_username(self, username):
        """
        Entrer le username dans le champ approprié
        
        Args:
            username: Nom d'utilisateur à saisir
        """
        # Attendre que l'input soit visible et interactif
        username_field = self.wait.until(
            EC.element_to_be_clickable(self.USERNAME_INPUT)
        )
        username_field.clear()
        username_field.send_keys(username)
    
    def enter_email(self, email):
        """
        Entrer l'email/username dans le champ approprié
        (Alias pour enter_username pour compatibilité avec tests existants)
        
        Args:
            email: Adresse email ou username à saisir
        """
        self.enter_username(email)
    
    def enter_password(self, password):
        """
        Entrer le mot de passe
        
        Args:
            password: Mot de passe à saisir
        """
        password_field = self.wait.until(
            EC.element_to_be_clickable(self.PASSWORD_INPUT)
        )
        password_field.clear()
        password_field.send_keys(password)
    
    def click_login_button(self):
        """Cliquer sur le bouton de connexion"""
        login_btn = self.wait.until(
            EC.element_to_be_clickable(self.LOGIN_BUTTON)
        )
        # Scroll vers le bouton pour s'assurer qu'il est visible
        self.driver.execute_script("arguments[0].scrollIntoView(true);", login_btn)
        login_btn.click()
    
    def login(self, username, password):
        """
        Effectuer une connexion complète
        
        Args:
            username: Nom d'utilisateur (ou email)
            password: Mot de passe
        """
        self.enter_username(username)
        self.enter_password(password)
        self.click_login_button()
    
    def is_error_message_displayed(self, timeout=5):
        """
        Vérifier si un message d'erreur est affiché
        
        Args:
            timeout: Temps d'attente maximum (secondes)
            
        Returns:
            bool: True si un message d'erreur est visible
        """
        try:
            wait = WebDriverWait(self.driver, timeout)
            error_element = wait.until(
                EC.visibility_of_element_located(self.ERROR_MESSAGE)
            )
            return error_element.is_displayed()
        except TimeoutException:
            return False
    
    def get_error_message(self):
        """
        Récupérer le texte du message d'erreur
        
        Returns:
            str: Texte du message d'erreur
        """
        try:
            error_element = self.driver.find_element(*self.ERROR_MESSAGE)
            return error_element.text
        except:
            return ''
    
    def is_login_successful(self, timeout=10):
        """
        Vérifier si la connexion a réussi (redirection)
        
        Args:
            timeout: Temps d'attente maximum (secondes)
            
        Returns:
            bool: True si redirigé vers dashboard/home ou admin
        """
        try:
            wait = WebDriverWait(self.driver, timeout)
            # Attendre que l'URL change et ne contienne plus /login
            wait.until(lambda d: '/login' not in d.current_url.lower())
            # Vérifier qu'on est bien redirigé vers une page valide
            current_url = self.driver.current_url.lower()
            return any(path in current_url for path in ['/admin', '/dashboard', '/home', '/', '/vehicles'])
        except TimeoutException:
            return False
    
    def get_current_url(self):
        """Retourner l'URL actuelle"""
        return self.driver.current_url
    
    def is_password_field_masked(self):
        """
        Vérifier si le champ mot de passe est masqué (type="password")
        
        Returns:
            bool: True si le champ est de type password
        """
        password_field = self.driver.find_element(*self.PASSWORD_INPUT)
        return password_field.get_attribute('type') == 'password'
