import React, { createContext, useContext, useState } from "react"
import {
  BrowserRouter as Router,
  Switch,
  Route as RouteBase,
  Redirect,
  Link,
  useHistory,
  useLocation,
  useRouteMatch
} from "react-router-dom"

const Key = "auth"

export const Provider = ({ value = initState(), children }) => {
  const [state, set] = useState(value)
  return <Store.Provider value={{ state, set }}>{children}</Store.Provider>
}

//TODO: failed
// 1. show dialog if valid = false
// {"userInfo":null,"valid":false,"message":"User not found.","errors":[]}

//TODO: ok
// {"userInfo":{"userId":"bbe47aab-0d5b-49ec-1391-08d84a6cdf33","username":"sam.chuang","timeZone":"+0800","role":"Admin","menus":[{"subMenus":[{"permissions":{"View":true,"Create":true,"Update":true,"ManageStatus":true,"CanEditCredential":true},"code":"Resellers","name":"Resellers","translation":"Resources.UI.Partial.Menu.Menu.LinkResellers","icon":null,"displayOrder":1},{"permissions":{"View":true,"Create":true,"Update":true,"ManageStatus":true},"code":"Brands","name":"Brands","translation":"Resources.UI.Partial.Menu.Menu.LinkBrands","icon":null,"displayOrder":2}],"code":"Reseller","name":"Reseller","translation":"Resources.UI.Partial.Menu.Menu.MenuTextReseller","icon":"fa-certificate","displayOrder":1},{"subMenus":[{"permissions":{"View":true,"Create":true,"Update":true,"CanEditCredential":true,"ManageStatus":true,"CanManageArchiveStatus":true,"CanManageConfigStatus":true,"CanViewGames":true},"code":"GameProviders","name":"Game Providers","translation":"Resources.UI.Partial.Menu.Menu.LinkGameProviders","icon":null,"displayOrder":1}],"code":"Game","name":"Game","translation":"Resources.UI.Partial.Menu.Menu.MenuTextGame","icon":"fa-gamepad","displayOrder":2},{"subMenus":[{"permissions":{"View":true},"code":"AdminLog","name":"Admin","translation":"Resources.UI.Partial.Menu.Menu.LinkAdmin","icon":null,"displayOrder":1},{"permissions":{"View":true},"code":"ApiLog","name":"API","translation":"Resources.UI.Partial.Menu.Menu.LinkAPI","icon":null,"displayOrder":2},{"permissions":{"View":true},"code":"SystemLog","name":"System","translation":"Resources.UI.Partial.Menu.Menu.LinkSystem","icon":null,"displayOrder":3}],"code":"Log","name":"Log","translation":"Resources.UI.Partial.Menu.Menu.MenuTextLog","icon":"fa-file-text-o","displayOrder":3},{"subMenus":[{"permissions":{"View":true,"Export":true,"CanViewBetHistory":true},"code":"Overall","name":"Overall","translation":"Resources.UI.Partial.Menu.Menu.LinkOverall","icon":null,"displayOrder":1},{"permissions":{"View":true,"Export":true,"CanViewBetHistory":true},"code":"ReportGameProvider","name":"GameProvider","translation":"Resources.UI.Partial.Menu.Menu.LinkGameProvider","icon":null,"displayOrder":2}],"code":"Report","name":"Report","translation":"Resources.UI.Partial.Menu.Menu.MenuTextReport","icon":"fa-bar-chart","displayOrder":4},{"subMenus":[{"permissions":{"View":true,"Create":true,"Update":true,"ManageStatus":true},"code":"Users","name":"Users","translation":"Resources.UI.Partial.Menu.Menu.LinkUsers","icon":null,"displayOrder":1},{"permissions":{"View":true,"Create":true,"Update":true},"code":"Currencies","name":"Currencies","translation":"Resources.UI.Partial.Menu.Menu.LinkCurrencies","icon":null,"displayOrder":2},{"permissions":{"View":true,"Create":true,"Update":true},"code":"Languages","name":"Languages","translation":"Resources.UI.Partial.Menu.Menu.LinkLanguages","icon":null,"displayOrder":3},{"permissions":{"View":true,"Create":true,"Update":true,"ManageStatus":true},"code":"Configurations","name":"Configurations","translation":"Resources.UI.Partial.Menu.Menu.LinkApplicationConfigurations","icon":null,"displayOrder":4},{"permissions":{"View":true,"Create":true,"Update":true,"Export":true},"code":"WhilelistIPs","name":"WhileLists IPs","translation":"Resources.UI.Partial.Menu.Menu.LinkWhitelists","icon":null,"displayOrder":5}],"code":"System","name":"System","translation":"Resources.UI.Partial.Menu.Menu.MenuTextSystem","icon":"fa-cog","displayOrder":5},{"subMenus":[{"permissions":{"View":true,"Create":true,"Update":true,"CanManageMenu":true,"ManageStatus":true,"Export":true,"Import":true},"code":"Roles","name":"Roles","translation":"Resources.UI.Partial.Menu.Menu.LinkRoles","icon":null,"displayOrder":1},{"permissions":{"View":true,"Create":true,"Update":true,"Delete":true},"code":"Menu","name":"Menu","translation":"Resources.UI.Partial.Menu.Menu.LinkMenu","icon":null,"displayOrder":2},{"permissions":{"View":true,"Create":true,"Update":true,"Delete":true},"code":"Permission","name":"Permission","translation":"Resources.UI.Partial.Menu.Menu.LinkPermission","icon":null,"displayOrder":3}],"code":"RolePermission","name":"Role And Permission","translation":"Resources.UI.Partial.Menu.Menu.MenuTextRoleAndPermission","icon":"fa-certificate","displayOrder":6}]},"valid":true,"message":"","errors":[]}
export const useLogin = () => {
  let history = useHistory()
  let location = useLocation()
  let DefaultRedirectState = { from: { pathname: "/" } }
  let { from } = location.state ?? DefaultRedirectState

  return (data = {}) =>
    fetch("api/users/auth", {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify(data)
    })
      .then((response) => {
        if (!response.ok) {
          throw new Error(response.statusText)
        }

        return response.json()
      })
      .then((data) => {
        set(data)
        history.replace(from)

        //TODO: check browser private mode, throw exception?
        localStorage.setItem(Key, JSON.stringify(data))
      })
}

export const useLogout = () => {
  let { set } = useContext(Store)
  return () => {
    //TODO: check browser private mode, throw exception?
    localStorage.removeItem(Key)

    set({})
  }
}

//CSRF token
export const useToken = () => {}

const initState = () => {
  //TODO: check browser private mode, throw exception?
  let value = localStorage.getItem(Key)
  if (!value) {
    return {}
  }

  //TODO: remove fake data
  //TODO: need try catch JSON.parse ?
  return JSON.parse('{ "token": "" }')
}
