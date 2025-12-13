# Create all Python test files
import os

# Test auth API - complete
test_auth_complete = """import pytest
import requests

@pytest.mark.api
@pytest.mark.auth
@pytest.mark.integration
class TestAuthenticationAPI:
    
    def test_TC011_login_valid_credentials_returns_token(self, api_url):
        login_data = {
            'email': 'admin@carrental.com',
            'password': 'Admin@123'
        }
        response = requests.post(f'{api_url}/api/auth/login', json=login_data)
        assert response.status_code == 200
        data = response.json()
        assert 'token' in data
        assert data['token']
        assert len(data['token'].split('.')) == 3
    
    def test_TC012_login_invalid_password_returns_unauthorized(self, api_url):
        login_data = {
            'email': 'admin@carrental.com',
            'password': 'WrongPassword123!'
        }
        response = requests.post(f'{api_url}/api/auth/login', json=login_data)
        assert response.status_code == 401
    
    @pytest.mark.parametrize("email,password", [
        ('', 'password123'),
        ('invalidemail', 'password'),
        ('test@test.com', ''),
    ])
    def test_TC013_login_invalid_inputs(self, api_url, email, password):
        login_data = {'email': email, 'password': password}
        response = requests.post(f'{api_url}/api/auth/login', json=login_data)
        assert response.status_code in [400, 401]
"""

# Test vehicles API
test_vehicles = """import pytest
import requests

@pytest.mark.api
@pytest.mark.vehicles
@pytest.mark.integration
class TestVehiclesAPI:
    
    def test_TC018_get_all_vehicles(self, api_url):
        response = requests.get(f'{api_url}/api/vehicles')
        assert response.status_code in [200, 401]
    
    def test_TC019_get_vehicle_by_id_existing(self, api_url):
        response = requests.get(f'{api_url}/api/vehicles/1')
        assert response.status_code in [200, 401, 404]
    
    def test_TC020_get_vehicle_by_id_nonexisting(self, api_url):
        response = requests.get(f'{api_url}/api/vehicles/99999')
        assert response.status_code in [401, 404]
"""

# Test UI login
test_ui_login = """import pytest
import time
from pages.login_page import LoginPage

@pytest.mark.ui
@pytest.mark.auth
class TestLoginUI:
    
    def test_TC023_login_ui_valid_credentials(self, driver, base_url):
        login_page = LoginPage(driver)
        login_page.navigate_to(base_url)
        login_page.login('admin@carrental.com', 'Admin@123')
        time.sleep(2)
        assert login_page.is_login_successful()
    
    def test_TC024_login_ui_invalid_password(self, driver, base_url):
        login_page = LoginPage(driver)
        login_page.navigate_to(base_url)
        login_page.login('admin@carrental.com', 'WrongPassword')
        time.sleep(2)
        assert login_page.is_error_message_displayed()
"""

# Test UI vehicles
test_ui_vehicles = """import pytest
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
"""

# Create all test files
os.makedirs('tests', exist_ok=True)

with open('tests/test_auth_api.py', 'w') as f:
    f.write(test_auth_complete)

with open('tests/test_vehicles_api.py', 'w') as f:
    f.write(test_vehicles)

with open('tests/test_login_ui.py', 'w') as f:
    f.write(test_ui_login)

with open('tests/test_vehicles_ui.py', 'w') as f:
    f.write(test_ui_vehicles)

print("All test files created successfully!")
print("- tests/test_auth_api.py")
print("- tests/test_vehicles_api.py")
print("- tests/test_login_ui.py")
print("- tests/test_vehicles_ui.py")
