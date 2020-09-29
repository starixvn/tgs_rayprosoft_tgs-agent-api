import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import Async, { useAsync, IfRejected, IfFulfilled } from "react-async"
import { useLocation, useRouteMatch } from "react-router-dom"
import client from "../../client"
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
import { Update as UpdateMenu } from "./menu"

export const EditButton = ({ user, onSubmitted, ...props }) => {
  let dialog = Modal.useState()
  let disclosureRef = useRef()

  let isDisplay = useIsDisplayMenuByRoute([UpdateMenu])
  if (!isDisplay) {
    return null
  }

  return (
    <>
      <Modal.Disclosure
        use={Button}
        ref={disclosureRef}
        css={css`
          flex: unset;
        `}
        {...dialog}
      >
        {edit()}
      </Modal.Disclosure>
      <EditDialog
        onSubmitted={onSubmitted}
        dialog={dialog}
        finalFocusRef={disclosureRef}
      />
    </>
  )
}

const EditDialog = () => {
  return null
}
