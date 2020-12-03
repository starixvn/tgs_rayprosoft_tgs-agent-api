import React, { useState, useEffect, useMemo } from "react"
import { css } from "@emotion/react"
import { Table, Set, Button, Box, Stack, Select, DropdownMenu } from "bumbag"
import { Switch, Route, Link, useParams, useRouteMatch } from "react-router-dom"
import { useGameProviderList } from "./client"

export const GameProviderDropdownMenu = ({
  brandCode,
  currency,
  brandToken,
  username,
  platform,
  language,
  value,
  ...props
}) => {
  let { items: gameProviders = [] } = useGameProviderList(
    { brandCode, currency, username },
    { platform, language },
    { brandToken }
  )

  let gameProvider = gameProviders.find(({ code }) => code === value)
  let menu = useMemo(
    () => (
      <>
        {gameProviders.map((gameProvider) => (
          <DropdownMenu.Item
            key={gameProvider.code}
            onClick={(event) => props?.onChange?.(gameProvider.code)}
          >
            {gameProvider.name}
          </DropdownMenu.Item>
        ))}
      </>
    ),
    [JSON.stringify(gameProviders)]
  )
  return (
    <DropdownMenu menu={menu}>
      <Button iconAfter="chevron-down">
        {gameProvider ? gameProvider.name : "Game Provider"}
      </Button>
    </DropdownMenu>
  )
}
