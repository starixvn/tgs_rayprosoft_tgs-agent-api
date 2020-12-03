import React, { useState, useEffect, createContext, useContext } from "react"
import { css } from "@emotion/react"
import { Table, Set, Button, Box, Stack, Select } from "bumbag"
import { Switch, Route, Link, useParams, useRouteMatch } from "react-router-dom"
import { Desktop } from "../platform"
import { CNY } from "../currency"
import { enUS, zhCN } from "../language"

const Store = createContext({})

export const State = ({ value, children }) => {
  let [state, set] = useState({
    platform: Desktop,
    currency: CNY,
    lang: zhCN,
    ...value
  })

  return (
    <Store.Provider
      value={{
        ...state,
        set
      }}
    >
      {children}
    </Store.Provider>
  )
}

export const usePlatform = () => {
  let { platform } = useContext(Store)
  return platform
}

export const useSetPlatform = () => {
  let { set } = useContext(Store)
  return (platform) => set((prevState) => ({ ...prevState, platform }))
}

export const useCurrency = () => {
  let { currency } = useContext(Store)
  return currency
}

export const useSetCurrency = () => {
  let { set } = useContext(Store)
  return (currency) => set((prevState) => ({ ...prevState, currency }))
}

export const useLang = () => {
  let { lang } = useContext(Store)
  return lang
}

export const useSetLang = () => {
  let { set } = useContext(Store)
  return (lang) => set((prevState) => ({ ...prevState, lang }))
}
