import React, { useState, useEffect } from "react"
import { css } from "@emotion/react"
import {
  Table,
  Set,
  Button,
  Stack,
  Box,
  PageWithHeader,
  TopNav,
  DropdownMenu,
  Group,
  Card
} from "bumbag"
import { currencyList } from "./index"

export const CurrencyDropdownMenu = ({ value, ...props }) => {
  let options = currencyList().filter((item) => item != value)
  return (
    <DropdownMenu
      menu={
        <>
          {options.map((currency) => (
            <DropdownMenu.Item
              key={currency}
              onClick={(event) => {
                props?.onChange(currency)
              }}
            >
              {currency}
            </DropdownMenu.Item>
          ))}
        </>
      }
    >
      <Button iconAfter="chevron-down">{value ? value : "Currency"}</Button>
    </DropdownMenu>
  )
}

export default CurrencyDropdownMenu
