import pytest
import time
from pages.login_page import LoginPage

@pytest.mark.ui
@pytest.mark.auth
class TestLoginUI:
    
    def test_TC023_login_ui_valid_credentials(self, driver, base_url):
        """Test login with valid admin credentials"""
        login_page = LoginPage(driver)
        login_page.navigate_to(base_url)
        # Wait for Blazor to load
        time.sleep(2)
        # Use the demo admin account shown in Login.razor
        login_page.login('admin', 'Admin@123')
        time.sleep(3)
        assert login_page.is_login_successful(), f"Login failed. Current URL: {login_page.get_current_url()}"
    
    def test_TC024_login_ui_invalid_password(self, driver, base_url):
        """Test login with invalid password shows error"""
        login_page = LoginPage(driver)
        login_page.navigate_to(base_url)
        # Wait for Blazor to load
        time.sleep(2)
        login_page.login('admin', 'WrongPassword')
        time.sleep(3)
        assert login_page.is_error_message_displayed(), "Error message should be displayed for invalid credentials"
