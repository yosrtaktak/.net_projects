from selenium.webdriver.common.by import By
from time import sleep
class LoginPage:
    textbox_username_id = "user-name"
    textbox_password_id = "password"
    button_login_xpath = "//input[@id='login-button']"
    button_menu="react-burger-menu-btn"
    link_logout = "logout_sidebar_link"
    def __init__(self, driver):
        self.driver = driver
    def setUserName(self, username):
        
        self.driver.find_element(By.ID, self.textbox_username_id).clear()
        self.driver.find_element(By.ID, self.textbox_username_id).send_keys(username)
    def setPassword(self, password):
        self.driver.find_element(By.ID, self.textbox_password_id).clear()
        self.driver.find_element(By.ID, self.textbox_password_id).send_keys(password)
    def clickLogin(self):
        self.driver.find_element(By.XPATH, self.button_login_xpath).click()  
    def clickLogout(self):
        self.driver.find_element(By.ID, self.button_menu).click()
        sleep(2)
        self.driver.find_element(By.ID, self.link_logout).click()