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

export const BrandHeader = ({ navItems = [] }) => {
  let currency = useCurrency()
  let setCurrency = useSetCurrency()

  return (
    <TopNav padding="major-1">
      <TopNav.Section>
        <TopNav.Item>
          <CurrencyDropdownMenu value={currency} onChange={setCurrency} />
        </TopNav.Item>
        {navItems}
      </TopNav.Section>
    </TopNav>
  )
}

export default BrandHeader
