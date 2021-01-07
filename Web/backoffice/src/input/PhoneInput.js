import React, { useState, useEffect, useMemo, useRef, Children } from "react"
import { css, Global } from "@emotion/react"
import { useForm, useFormContext } from "react-hook-form"
import { useTrans } from "../i18n"
import HookFormInputField from "./Input"

const PhoneReg = /^\+(?:[0-9] ?){6,14}[0-9]$/ //Refer to https://www.oreilly.com/library/view/regular-expressions-cookbook/9781449327453/ch04s03.html
export const PhoneInput = ({ type = "tel", name = "phone", ...props }) => {
  //TODO: error msg popup?
  let invalidPhoneNumber = useTrans({
    zh: "无效电话",
    en: "Invalid phone number"
  })

  return (
    <HookFormInputField
      name={name}
      /*
      validate={(value) => {
        
        if (!PhoneReg.test(value)) {
          return false// invalid phone number
        }
        
        return true
      }}
      */
      {...props}
    />
  )
}

export default PhoneInput
