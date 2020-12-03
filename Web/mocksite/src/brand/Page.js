import React, { useState, useEffect } from "react"
import { css } from "@emotion/react"
import { Table, Set, Button, Box, Stack, PageWithHeader, TopNav } from "bumbag"
import {
  Switch,
  Route,
  Link,
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
  //TODO: how to check token expires
  //TODO: 1. select currency or set default currency? -> list players
  return (
    <State>
      <Body brandCode={brandCode} />
    </State>
  )
}

const Body = ({ brandCode }) => {
  let { path, url } = useRouteMatch()
  let history = useHistory()
  let currency = useCurrency()
  let { token, expires } = useBrandToken({ brandCode, currency })

  useEffect(() => {
    history.push(`${url}/${currency}`)
  }, [currency])

  return (
    <Switch>
      <Route path={`${path}/:currency`}>
        <PlayerList brandToken={token} />
      </Route>
    </Switch>
  )
}

export default BrandPage
