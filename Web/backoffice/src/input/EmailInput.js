import React, { useState, useEffect, useMemo, useRef, Children } from "react"
import { css, Global } from "@emotion/react"
import { useForm, useFormContext } from "react-hook-form"
import { useTrans } from "../i18n"
import HookFormInputField from "./Input"

const EmailReg = /^[a-zA-Z0-9.!#$%&'*+\/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$/

export const EmailInput = ({ type = "mail", name = "email", ...props }) => {
  //TODO: error msg popup?
  let invalidEmail = useTrans({
    zh: "无效邮箱",
    en: "Invalid email"
  })

  return (
    <HookFormInputField
      name={name}
      validate={(value) => {
        if (value && !EmailReg.test(value)) {
          return false
        }

        return true
      }}
    />
  )
}

export default EmailInput
