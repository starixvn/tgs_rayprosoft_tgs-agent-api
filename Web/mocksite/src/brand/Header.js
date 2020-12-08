import React, { useState, useEffect } from "react"
import { css } from "@emotion/react"
import {
  Table,
  Set,
  Button,
  Box,
  Stack,
  PageWithHeader,
  TopNav,
  Popover,
  Input,
  InputField,
  FieldStack,
  Text,
  FieldWrapper
} from "bumbag"
import {
  Switch,
  Route,
  Link,
  useHistory,
  useParams,
  useRouteMatch
} from "react-router-dom"
import { useBrandToken } from "../client/brand"
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
  let { brandCode } = useParams()
  let { token = "", expires } = useBrandToken({ brandCode, currency })

  let now = new Date().getTime()
  let isExpired = now > expires

  return (
    <TopNav padding="major-1">
      <TopNav.Section>
        <TopNav.Item>{brandCode}</TopNav.Item>
        <TopNav.Item>
          <CurrencyDropdownMenu value={currency} onChange={setCurrency} />
        </TopNav.Item>
        {navItems}
      </TopNav.Section>
    </TopNav>
  )
}

export default BrandHeader
