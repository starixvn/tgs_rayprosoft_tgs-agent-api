import React, { useState, useEffect } from "react"
import { css } from "@emotion/react"
import { Table, Set, Button, Box, Stack, PageWithHeader, TopNav } from "bumbag"
import {
  Switch,
  Route,
  Link,
  useLocation,
  useHistory,
  useParams,
  useRouteMatch
} from "react-router-dom"
import { useBrandToken } from "./client"
import {
  State,
  useCurrency,
  usePlatform,
  useSetCurrency,
  useSetPlatform
} from "./State"
import CurrencyDropdownMenu from "../currency/DropdownMenu"
import BrandHeader from "./Header"
import { PlayerList } from "./player/List"

export const BrandPage = ({}) => {
  let { brandCode } = useParams()
  return (
    <State>
      <Body brandCode={brandCode} />
    </State>
  )
}

const Body = ({ brandCode }) => {
  let location = useLocation()
  let { path, url } = useRouteMatch()
  let history = useHistory()
  let currency = useCurrency()
  let { token = "", expires } = useBrandToken({ brandCode, currency })

  useEffect(() => {
    if (url === location.pathname) {
      history.push(`${url}/${currency}`)
    }
  }, [currency, url, location.pathname])

  return (
    <Switch>
      <Route path={`${path}/:currency`}>
        <PlayerList brandToken={token} />
      </Route>
    </Switch>
  )
}

export default BrandPage
