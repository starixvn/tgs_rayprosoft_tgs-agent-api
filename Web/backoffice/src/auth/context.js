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

export const useLogin = () => {
  let history = useHistory()
  let location = useLocation()
  let DefaultRedirectState = { from: { pathname: "/" } }
  let { from } = location.state ?? DefaultRedirectState

  return ({ username, password }) =>
    fetch("api/status", {
      headers: {
        "Content-Type": "application/json",
        Host: "backoffice"
      }
    })
      .then((response) => {
        if (!response.ok) {
          throw new Error(response.statusText)
        }

        return response.json()
      })
      .then((data) => {
        console.log(
          "login user=",
          username,
          ", pwd =",
          password,
          "response data",
          data
        )

        //TODO: check browser private mode, throw exception?
        localStorage.setItem(Key, JSON.stringify(data))

        set(data)

        history.replace(from)
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
