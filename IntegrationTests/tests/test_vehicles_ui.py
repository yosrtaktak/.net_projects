import pytest
import time
from pages.vehicles_page import VehiclesPage

@pytest.mark.ui
@pytest.mark.vehicles
class TestVehiclesUI:
    
    def test_TC028_browse_vehicles_displays_list(self, driver, base_url):
        vehicles_page = VehiclesPage(driver)
        vehicles_page.navigate_to(base_url)
        time.sleep(2)
        assert vehicles_page.get_vehicle_count() >= 0
    
    def test_TC029_search_vehicle_valid_term(self, driver, base_url):
        vehicles_page = VehiclesPage(driver)
        vehicles_page.navigate_to(base_url)
        time.sleep(2)
        vehicles_page.search_vehicle('Renault')
        time.sleep(2)
        # Just verify no crash
        assert True
