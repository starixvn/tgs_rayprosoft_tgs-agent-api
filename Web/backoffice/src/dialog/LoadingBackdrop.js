import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/core"
import {
  Spinner,
  Stack,
  Flex,
  Box,
  Card,
  Set,
  Button,
  Text,
  Label,
  Input,
  InputField,
  Checkbox,
  FieldStack,
  SelectField,
  Dialog,
  Modal,
  Overlay,
  Portal
} from "bumbag"

export default ({ state, ...props }) => {
  return (
    <Modal.State {...state}>
      <Modal.Backdrop {...props}>
        <Box alignY="center" height="100%">
          <Box alignX="center">
            <Spinner size="large" />
          </Box>
        </Box>
      </Modal.Backdrop>
    </Modal.State>
  )
}
