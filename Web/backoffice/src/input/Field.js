import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import Async, { useAsync, IfRejected, IfFulfilled } from "react-async"
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
  Checkbox,
  FieldStack,
  FieldWrapper,
  SelectField,
  Dialog,
  Modal,
  Table,
  useClassName
} from "bumbag"

export default ({ name, required, ...props }) => {
  return (
    <Field
      name={name}
      validate={(value) => {
        if (required && !value) {
          return "required"
        }
      }}
    >
      {({ field, form, meta }) => {
        let state = meta.error && meta.touched && "danger"
        return (
          <FieldWrapper state={state}>
            <Input id={name} {...field} {...props} />
          </FieldWrapper>
        )
      }}
    </Field>
  )
}
