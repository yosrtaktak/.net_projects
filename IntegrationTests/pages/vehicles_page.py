# -*- coding: utf-8 -*-
"""
Page Object Model pour la page des vehicules
Pattern: Page Object Model (POM)
"""
from selenium.webdriver.common.by import By
from selenium.webdriver.common.keys import Keys
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from selenium.common.exceptions import TimeoutException, NoSuchElementException


class VehiclesPage:
    """
    Page Object pour la page de navigation des vehicules
    Encapsule toutes les interactions avec la page /vehicles ou /browse
    """
    
    # Localisateurs
    VEHICLE_CARDS = (By.CSS_SELECTOR, '.vehicle-card, .mud-card, [data-testid="vehicle-card"]')
    SEARCH_INPUT = (By.CSS_SELECTOR, '#search-input, input[type="search"], input[placeholder*="Recherch"]')
    SEARCH_BUTTON = (By.CSS_SELECTOR, 'button[type="search"], button:has(.search-icon)')
    FILTER_DROPDOWN = (By.CSS_SELECTOR, '#category-filter, select[name="category"]')
    VIEW_DETAILS_BUTTONS = (By.CSS_SELECTOR, '.view-details-btn, button:contains("Details"), a[href*="/vehicle/"]')
    NO_RESULTS_MESSAGE = (By.CSS_SELECTOR, '.no-results, .empty-message, .no-vehicles')
    LOADING_INDICATOR = (By.CSS_SELECTOR, '.loading, .spinner, .mud-progress-circular')
    
    def __init__(self, driver):
        """
        Initialiser la VehiclesPage
        
        Args:
            driver: Instance du WebDriver Selenium
        """
        self.driver = driver
        self.wait = WebDriverWait(driver, 10)
    
    def navigate_to(self, base_url):
        """Naviguer vers la page des vehicules"""
        self.driver.get(f'{base_url}/vehicles')
    
    def wait_for_page_load(self, timeout=10):
        """Attendre que la page soit completement chargee"""
        try:
            # Attendre que le loading disparaisse
            wait = WebDriverWait(self.driver, timeout)
            wait.until_not(
                EC.presence_of_element_located(self.LOADING_INDICATOR)
            )
        except TimeoutException:
            pass  # Pas de loading indicator, continuer
    
    def get_vehicle_count(self):
        """
        Compter le nombre de vehicules affiches
        
        Returns:
            int: Nombre de cartes de vehicules visibles
        """
        try:
            vehicles = self.wait.until(
                EC.presence_of_all_elements_located(self.VEHICLE_CARDS)
            )
            return len(vehicles)
        except TimeoutException:
            return 0
    
    def are_vehicles_displayed(self):
        """
        Verifier si des vehicules sont affiches
        
        Returns:
            bool: True si au moins un vehicule est visible
        """
        return self.get_vehicle_count() > 0
    
    def search_vehicle(self, search_term):
        """
        Effectuer une recherche de vehicule
        
        Args:
            search_term: Terme de recherche
        """
        try:
            search_input = self.wait.until(
                EC.presence_of_element_located(self.SEARCH_INPUT)
            )
            search_input.clear()
            search_input.send_keys(search_term)
            
            # Essayer de cliquer sur le bouton de recherche
            try:
                search_button = self.driver.find_element(*self.SEARCH_BUTTON)
                search_button.click()
            except NoSuchElementException:
                # Si pas de bouton, envoyer Enter
                search_input.send_keys(Keys.ENTER)
            
            # Attendre que la recherche se termine
            import time
            time.sleep(1)
            
        except TimeoutException:
            print('Champ de recherche non trouve')
    
    def filter_by_category(self, category):
        """
        Filtrer les vehicules par categorie
        
        Args:
            category: Nom de la categorie
        """
        try:
            from selenium.webdriver.support.ui import Select
            filter_dropdown = self.wait.until(
                EC.presence_of_element_located(self.FILTER_DROPDOWN)
            )
            select = Select(filter_dropdown)
            select.select_by_visible_text(category)
            
            # Attendre que le filtrage se termine
            import time
            time.sleep(1)
            
        except TimeoutException:
            print('Dropdown de categorie non trouve')
    
    def click_first_vehicle_details(self):
        """Cliquer sur le premier vehicule pour voir les details"""
        try:
            details_buttons = self.wait.until(
                EC.presence_of_all_elements_located(self.VIEW_DETAILS_BUTTONS)
            )
            if details_buttons:
                details_buttons[0].click()
                # Attendre la navigation
                import time
                time.sleep(1)
        except TimeoutException:
            # Essayer de cliquer directement sur la premiere carte
            try:
                vehicle_cards = self.driver.find_elements(*self.VEHICLE_CARDS)
                if vehicle_cards:
                    vehicle_cards[0].click()
            except:
                print('Impossible de cliquer sur les details du vehicule')
    
    def is_no_results_message_displayed(self):
        """
        Verifier si le message "aucun resultat" est affiche
        
        Returns:
            bool: True si le message est visible
        """
        try:
            message = self.wait.until(
                EC.visibility_of_element_located(self.NO_RESULTS_MESSAGE)
            )
            return message.is_displayed()
        except TimeoutException:
            return False
    
    def get_vehicle_titles(self):
        """
        Recuperer les titres de tous les vehicules affiches
        
        Returns:
            list: Liste des titres de vehicules
        """
        try:
            vehicles = self.driver.find_elements(*self.VEHICLE_CARDS)
            return [vehicle.text for vehicle in vehicles]
        except:
            return []
    
    def get_first_vehicle_info(self):
        """
        Recuperer les informations du premier vehicule
        
        Returns:
            dict: Informations du vehicule (make, model, price, etc.)
        """
        try:
            first_card = self.driver.find_element(*self.VEHICLE_CARDS)
            return {
                'text': first_card.text,
                'displayed': first_card.is_displayed()
            }
        except:
            return {}
