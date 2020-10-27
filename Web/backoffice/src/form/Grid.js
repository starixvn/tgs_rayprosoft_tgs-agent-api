import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import { Formik, Field, Form, useFormikContext } from "formik"
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
  FieldWrapper,
  SelectField,
  Dialog,
  Modal,
  Table,
  useClassName
} from "bumbag"
import Grid from "../layout/Grid"

export const AreaLabelBox = ({ name = "", children, ...props }) => {
  return (
    <Box
      css={css`
        grid-area: ${name};
      `}
      margin="major-1"
    >
      <Label
        htmlFor={name}
        marginTop="major-1"
        css={css`
          line-height: unset;
        `}
      >
        {children}
      </Label>
    </Box>
  )
}

export const AreaFieldBox = ({ name = "", children, ...props }) => {
  return (
    <Box
      css={css`
        grid-area: ${name};
      `}
      margin="major-1"
    >
      {children}
    </Box>
  )
}

export const GridForm = ({ areas = "", children, ...props }) => {
  return (
    <Form>
      <Grid areas={areas}>{children}</Grid>
    </Form>
  )
}

export default GridForm
