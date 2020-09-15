import React, {
  createContext,
  useContext,
  useState,
  useMemo,
  useCallback
} from "react"
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
import client from "../client"
import memoize from "fast-memoize"

const Key = "user"

const InitialState = {
  userInfo: null
}

const Store = createContext({
  state: {}
})

export const Provider = ({ value = initState(), children }) => {
  const [state, set] = useState(value)
  return <Store.Provider value={{ state, set }}>{children}</Store.Provider>
}

export const useAuth = () => {
  let context = useContext(Store)
  let { set } = context
  let history = useHistory()
  let location = useLocation()
  let DefaultRedirectState = { from: { pathname: "/" } }
  let { from } = location.state ?? DefaultRedirectState

  return (data = {}) =>
    client
      .post("api/auth", {
        data
      })
      .then(({ userInfo, ..._ }) => {
        let newState = { userInfo }
        set(newState)
        history.replace(from)

        try {
          localStorage.setItem(Key, JSON.stringify(newState))
        } catch (error) {
          //ignore
        }
      })
}

export const useDeauth = () => {
  let { set } = useContext(Store)
  return () => {
    set(InitialState)

    try {
      localStorage.removeItem(Key)
    } catch (error) {
      //ignore
    }
  }
}

export const useIsAuthed = () => {
  let { state = {} } = useContext(Store)
  let userId = state?.userInfo?.userId
  return !!userId
}

export const useAuthCodes = (codes = []) => {
  let { state = {} } = useContext(Store)
  let menus = state?.userInfo?.menus ?? []

  return findCodes(codes, menus)
}

const findCodes = memoize((targetCodePaths = [], list = []) => {
  let items = targetCodePaths.reduce((list, code) => {
    code = code.toLowerCase()
    list =
      list.find((item) => item?.code?.toLowerCase() === code)?.subMenus ?? []

    return list
  }, list)

  let result = items.map((item) => item.code.toLowerCase())
  return result
})

const initState = () => {
  try {
    let stringValue = localStorage.getItem(Key)
    return stringValue ? JSON.parse(stringValue) : InitialState
  } catch (error) {
    //ignore
  }

  return InitialState
}
