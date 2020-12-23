import React, { useState, useEffect, useMemo } from "react"
import {
  css,
  Stack,
  Heading,
  Box,
  Button,
  Select,
  SelectMenu,
  DropdownMenu
} from "bumbag"
import { useTrans, Trans } from "../../i18n"

const Desktop = "Desktop"
const DesktopTrans = {
  zh: "PC",
  en: "PC"
}

const Mobile = "Mobile"
const MobileTrans = {
  zh: "M",
  en: "M"
}

export const GamePlatformLabel = ({ value }) => {
  switch (value) {
    case Desktop:
      return <Trans {...DesktopTrans} />

    case Mobile:
      return <Trans {...MobileTrans} />

    default:
      return value
  }
}
