import React, { useState, useEffect } from "react"
import { css, Global } from "@emotion/react"
import {
  Heading,
  PageContent,
  Box,
  Stack,
  Table,
  Card,
  Button,
  InputField,
  Set,
  Divider
} from "bumbag"

export default ({ title, actionBar, children, ...props }) => {
  return (
    <PageContent maxWidth="unset" {...props}>
      <Stack orientation="vertical" spacing="major-2">
        {title && (
          <Box marginBottom="major-3">
            <Heading use="h3">{title}</Heading>
          </Box>
        )}
        {actionBar && <Stack orientation="horizontal">{actionBar}</Stack>}
        {children}
      </Stack>
    </PageContent>
  )
}
