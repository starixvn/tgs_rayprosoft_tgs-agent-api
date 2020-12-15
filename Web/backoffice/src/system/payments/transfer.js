import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Select as _Select } from "bumbag"
import { Field as _Field } from "../../select/Field"
import { Trans, useTrans } from "../../i18n"

export const Deposit = 0
export const DepositString = "Deposit"

export const Withdrawal = 1
export const WithdrawalString = "Withdrawal"

export const Type = {
  Deposit: DepositString,
  Withdrawal: WithdrawalString
}

// string -> number
export const from = (value) => {
  switch (value) {
    case DepositString:
      return Deposit
    case WithdrawalString:
      return Withdrawal
    default:
      return ""
  }
}

/*
// number to string
export const toString = (value) => {}
*/
