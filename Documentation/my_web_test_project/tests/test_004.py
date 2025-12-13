import pytest
from pages.LoginPage import LoginPage  
from utilities.readProperties import ReadConfig
from selenium.webdriver.common.by import By
from utilities import XLUtils
import time
class Test_004_DDT_Login:
    baseURL = ReadConfig.getApplicationURL()
    path = "./TestData/LoginData.xlsx"
    def test_login_ddt(self,setup):
        self.driver = setup
        self.driver.get(self.baseURL)
        self.driver.maximize_window()
        self.lp = LoginPage(self.driver)
        self.rows = XLUtils.getRowCount(self.path, 'Sheet1')
        print('Number of rows...',self.rows)
        lst_status=[]

        for r in range(2,self.rows+1):
            self.user=XLUtils.readData(self.path,'Sheet1',r,1)
            self.password = XLUtils.readData(self.path, 'Sheet1', r, 2)
            self.exp = XLUtils.readData(self.path, 'Sheet1', r, 3)

            self.lp.setUserName(self.user)
            self.lp.setPassword(self.password)
            self.lp.clickLogin()
            time.sleep(5)

            act_title=self.driver.current_url
            exp_title="https://www.saucedemo.com/inventory.html"

            if act_title==exp_title:
                if self.exp=='Pass':
                    self.lp.clickLogout()
                    lst_status.append("Pass")
                elif self.exp=='Fail':
                    self.lp.clickLogout()
                    lst_status.append("Fail")

            elif act_title!=exp_title:
                if self.exp == 'Pass':
                    lst_status.append("Fail")
                elif self.exp == 'Fail':
                    lst_status.append("Pass")
            print(lst_status)
        if "Fail" not in lst_status:
            self.driver.close()
            assert True
        else:
            self.driver.close()
            assert False

