import pytest
from time import sleep
from pages.vehicles_page import VehiclesPage
from utilities.readProperties import ReadConfig
from utilities.customLogger import LogGen


class Test_002_Vehicles:
    """
    Classe de tests pour la page des vehicules.
    Suit le pattern de my_test avec ReadConfig et CustomLogger.
    """
    
    # Lecture des configurations
    baseURL = ReadConfig.getBaseURL()
    
    # Initialisation du logger
    logger = LogGen("./Logs/automation.log")
    
    @pytest.mark.ui
    @pytest.mark.vehicles
    def test_TC028_browse_vehicles_displays_list(self, setup):
        """Test d'affichage de la liste des vehicules"""
        self.logger.log_info("*************** Test_002_Vehicles ***************")
        self.logger.log_info("**** Started Browse Vehicles Test ****")
        
        self.driver = setup
        self.logger.log_info(f"**** Opening URL: {self.baseURL}/vehicles/browse ****")
        self.driver.get(f"{self.baseURL}/vehicles/browse")
        sleep(5)  # Attendre le chargement des vehicules
        
        # Création de l'instance de VehiclesPage
        self.vehiclesPage = VehiclesPage(self.driver)
        
        # Vérifier qu'au moins un vehicule est affiché
        vehicles_count = self.vehiclesPage.getVehicleCardsCount()
        self.logger.log_info(f"**** Found {vehicles_count} vehicles ****")
        
        if vehicles_count > 0:
            self.logger.log_info("**** Browse vehicles test passed ****")
            self.driver.close()
            assert True
        else:
            self.logger.log_error("**** Browse vehicles test failed - No vehicles found ****")
            self.driver.save_screenshot("./Screenshots/test_browse_vehicles.png")
            self.driver.close()
            assert False
    
    @pytest.mark.ui
    @pytest.mark.vehicles
    def test_TC029_search_vehicle_valid_term(self, setup):
        """Test de recherche de vehicule avec terme valide"""
        self.logger.log_info("**** Started Search Vehicle Test ****")
        
        self.driver = setup
        self.logger.log_info(f"**** Opening URL: {self.baseURL}/vehicles/browse ****")
        self.driver.get(f"{self.baseURL}/vehicles/browse")
        sleep(5)  # Attendre le chargement
        
        # Création de l'instance de VehiclesPage
        self.vehiclesPage = VehiclesPage(self.driver)
        
        # Eff
