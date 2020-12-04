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
import { platformList } from "./index"

export const PlatformDropdownMenu = ({ value, ...props }) => {
  let options = platformList().filter((platform) => platform != value)

  return (
    <DropdownMenu
      menu={
        <>
          {options.map((platform) => (
            <DropdownMenu.Item
              key={platform}
              onClick={(event) => {
                props?.onChange(platform)
              }}
            >
              {platform}
            </DropdownMenu.Item>
          ))}
        </>
      }
    >
      <Button iconAfter="chevron-down">{value ? value : "Platform"}</Button>
    </DropdownMenu>
  )
}

export default PlatformDropdownMenu
