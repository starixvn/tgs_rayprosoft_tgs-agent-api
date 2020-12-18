import React, {
  useState,
  useEffect,
  useMemo,
  useRef,
  createContext,
  useContext
} from "react"
import { css, Global } from "@emotion/react"
import { useForm } from "react-hook-form"
import { Dialog, Button, Box } from "bumbag"

export const HookFormDialog = ({ onSubmit, children, ...props }) => {
  const { register, handleSubmit, watch, errors } = useForm()
  return (
    <Dialog.Modal
      wrap={(children) => (
        <form onSubmit={handleSubmit(onSubmit)}>{children}</form>
      )}
      {...props}
    >
      {children}
    </Dialog.Modal>
  )
}
