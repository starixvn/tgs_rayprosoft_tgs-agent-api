import React, { useState, useEffect } from "react"
import { css } from "@emotion/react"
import { Table, Set, Button, Box, Stack, Select } from "bumbag"
import { Switch, Route, Link, useParams, useRouteMatch } from "react-router-dom"
import { useGameProviderList } from "./client"

export const GameProviderSelect = ({
  brandCode,
  username,
  currency,
  ...props
}) => {
  //TODO: currency?
  let result = useGameProviderList({ brandCode, username, currency })
  let options = []
  return <Box>TODO: {JSON.stringify(result)}</Box>
}

export default GameProviderSelect
