import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/core"
import Async from "react-async"
import client from "../../client"
import PageContent from "../../page/Content"
import AsyncTable, { useTableState } from "../../table/async"
import {
  useIsDisplayMenuByRoute,
  useHasSubMenuByRoute
} from "../../user/context"
import { Code } from "./menu"

export { Code }

export const Content = () => {
  return <PageContent>TODO: station letter</PageContent>
}
