import React, { useState, useEffect } from "react"
import { css, Global } from "@emotion/core"
import {
  Heading,
  PageContent,
  Stack,
  Table,
  Button,
  InputField,
  Set
} from "bumbag"

export default ({ title, actionBar, children, ...props }) => {
  //TODO: support actionBar fn?
  return (
    <PageContent css={css``}>
      <Stack orientation="vertical" spacing="major-2">
        {title && <Heading use="h3">{title}</Heading>}
        {actionBar && <Stack orientation="horizontal">{actionBar}</Stack>}
      </Stack>
      {children}
    </PageContent>
  )
}
