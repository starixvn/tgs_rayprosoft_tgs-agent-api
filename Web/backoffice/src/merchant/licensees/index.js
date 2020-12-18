import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import { Box, Set, Button, Text, Switch, Input, Stack } from "bumbag"
import PageContent from "../../page/Content"
import { useTrans } from "../../i18n"
import {
  useIsDisplayMenuByRoute,
  useHasSubMenuByRoute
} from "../../user/context"
import { Code, Update as UpdateMenu } from "./menu"

export { Code }

export const Licensee = "Licensee"

export const LicenseeTrans = {
  zh: "公司",
  en: "Licensee"
}

export const useLicenseeLabel = () => useTrans(LicenseeTrans)

export const Content = () => {
  return ""
}
