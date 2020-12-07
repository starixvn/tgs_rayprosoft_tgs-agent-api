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
  let { brandCode } = useParams()
  let { token = "", expires } = useBrandToken({ brandCode, currency })

  let now = new Date().getTime()
  let isExpired = now > expires
  console.log("TODO: update isExpired", isExpired)
  return (
    <TopNav padding="major-1">
      <TopNav.Section>
        <TopNav.Item>
          <Popover.State>
            <Popover.Disclosure
              use={Button}
              palette={isExpired ? "danger" : "success"}
              variant="outlined"
            >
              {brandCode}
            </Popover.Disclosure>
            <Popover hasArrow maxWidth="650px">
              <Stack>
                <Text.Block>
                  {new Date(expires).toLocaleString()}{" "}
                  {`Expired: ${isExpired.toLocaleString()}`}
                </Text.Block>
              </Stack>
            </Popover>
          </Popover.State>
        </TopNav.Item>
        <TopNav.Item>
          <CurrencyDropdownMenu value={currency} onChange={setCurrency} />
        </TopNav.Item>
        {navItems}
      </TopNav.Section>
    </TopNav>
  )
}

export default BrandHeader
