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
import { useBrandToken } from "../client/brand"
import { State, useCurrency, usePlayerType, useSetCurrency } from "./State"
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
        let brandCurrencyPath = `${url}/${currency}`
        if (!location.pathname.startsWith(brandCurrencyPath)) {
            history.push(brandCurrencyPath)
        }
    }, [currency, url, location.pathname])

    return (
        <Switch>
            <Route path={`${path}/:currency`}>
                <Content brandToken={token} />
            </Route>
        </Switch>
    )
}

const Content = ({ brandToken }) => {
    let location = useLocation()
    let { path, url } = useRouteMatch()
    let history = useHistory()

    let playerType = usePlayerType()

    useEffect(() => {
        let path = `${url}/${playerType}`
        if (!location.pathname.startsWith(path)) {
            history.push(path)
        }
    }, [playerType, url, location.pathname])

    return (
        <Switch>
            <Route path={`${path}/:playerType`}>
                <PlayerList brandToken={brandToken} />
            </Route>
        </Switch>
    )
}

export default BrandPage
