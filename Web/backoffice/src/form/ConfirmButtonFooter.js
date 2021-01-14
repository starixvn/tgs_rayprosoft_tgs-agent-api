import React, {
  useState,
  useEffect,
  useMemo,
  useRef,
  createContext,
  useContext
} from "react"
import { css, Global } from "@emotion/react"
import { useForm, FormProvider, useFormContext } from "react-hook-form"
import { Dialog, Button, Box } from "bumbag"
import { CancelText, CloseText, SubmitText } from "../button/ActionButtons"
import AlertDialog from "../dialog/Alert"

export const ConfirmButtonFooter = ({
  isLoading,
  disabled,
  width = "100%",
  alignX = "center",
  ...props
}) => {
  return (
    <Box width={width} alignX={alignX} {...props}>
      <Button
        palette="primary"
        type="submit"
        isLoading={isLoading}
        disabled={disabled}
      >
        <SubmitText />
      </Button>
    </Box>
  )
}

export const CloseButtonFooter = ({
  width = "100%",
  alignX = "center",
  dialog,
  ...props
}) => {
  return (
    <Box width={width} alignX={alignX} {...props}>
      <Button palette="primary" type="button" onClick={dialog?.hide}>
        <CloseText />
      </Button>
    </Box>
  )
}

export default ConfirmButtonFooter
