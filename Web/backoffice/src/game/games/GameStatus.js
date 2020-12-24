import React, { useState, useEffect, useMemo } from "react"
import {
  css,
  Stack,
  Heading,
  Box,
  Button,
  Select,
  SelectMenu,
  DropdownMenu,
  Switch
} from "bumbag"
import { useTrans, Trans } from "../../i18n"

const Active = "Active"
const Inactive = "Inactive"
const Archived = "Archived"
const Maintenance = "Maintenance"
const Testing = "Testing"

export const GameStatus = {
  Active,
  Inactive
}

export const GameStatusToggleSwitch = ({ value }) => {
  let isActive = value == Active
  return <Switch checked={isActive} />
}
