# -*- coding: utf-8 -*-
"""
Page Object Model pour la page des vehicules
Pattern: Page Object Model (POM)
"""
from selenium.webdriver.common.by import By
from time import sleep


class VehiclesPage:
    """
    Page Object pour la page des vehicules.
    Encapsule toutes les interactions avec la page /vehicles.
    """
    
    # Localisateurs
    vehicle_card_css = '.mud-card, .vehicle-card'
    search_input_css = 'input[type="search"], input[placeholder*="Search"], input[placeholder*="Rechercher"]'
    vehicle_title_css = '.mud-card-header, .vehicle-title, h6'
    
    def __init__(self, driver):
        """
        Initialiser la VehiclesPage.
        
        Args:
            driver: Instance du WebDriver Selenium
        """
        self.driver = driver
    
    def getVehicleCardsCount(self):
        """
        Obtenir le nombre de cartes de vehicules affichees.
        
        Returns:
            int: Nombre de vehicules affiches
        """
        try:
            cards = self.driver.find_elements(By.CSS_SELECTOR, self.vehicle_card_css)
            return len(cards)
        except:
            return 0
    
    def searchVehicle(self, search_term):
        """
        Rechercher un vehicule.
        
        Args:
            search_term: Terme de recherche
        """
        try:
            search_input = self.driver.find_element(By.CSS_SELECTOR, self.search_input_css)
            search_input.clear()
            search_input.send_keys(search_term)
            sleep(1)  # Attendre que la recherche se complete
        except:
            pass
    
    def isVehicleDisplayed(self, vehicle_name):
        """
        Verifier si un vehicule specifique est affiche.
        
        Args:
            vehicle_name: Nom du vehicule a chercher
            
        Returns:
            bool: True si le vehicule est trouve
        """
        try:
            titles = self.driver.find_elements(By.CSS_SELECTOR, self.vehicle_title_css)
            for title in titles:
                if vehicle_name.lower() in title.text.lower():
                    return True
            return False
        except:
            return False
    
    def getCurrentURL(self):
        """Retourner l'URL actuelle"""
        return self.driver.current_url
