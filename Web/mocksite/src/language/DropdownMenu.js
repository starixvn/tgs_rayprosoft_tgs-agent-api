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
import { langList } from "./index"

export const LangDropdownMenu = ({ value, ...props }) => {
  let options = langList().filter((lang) => lang != value)

  return (
    <DropdownMenu
      menu={
        <>
          {options.map((lang) => (
            <DropdownMenu.Item
              key={lang}
              onClick={(event) => {
                props?.onChange?.(lang)
              }}
            >
              {lang}
            </DropdownMenu.Item>
          ))}
        </>
      }
    >
      <Button iconAfter="chevron-down">{value ? value : "Language"}</Button>
    </DropdownMenu>
  )
}

export default LangDropdownMenu
