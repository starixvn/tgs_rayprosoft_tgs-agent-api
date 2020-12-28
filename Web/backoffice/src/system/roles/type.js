import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Trans, useTrans } from "../../i18n"
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
  Select,
  SelectField,
  Dialog,
  Modal,
  Table,
  useClassName
} from "bumbag"
import { useUserRoleType } from "../../user/context"
import { Reseller, ResellerTrans } from "../../merchant/resellers"
import { Licensee, LicenseeTrans } from "../../merchant/licensees"
import { Brand, BrandTrans } from "../../merchant/brands"

const Admin = "Admin"
const AdminTrans = {
  zh: "管理员",
  en: "Admin"
}

export const RoleTypes = {
  Admin,
  Reseller,
  Licensee,
  Brand
}

export const RoleTypeLabel = ({ value }) => {
  switch (value) {
    case Admin:
      return <Trans {...AdminTrans} />
    case Reseller:
      return <Trans {...ResellerTrans} />
    case Licensee:
      return <Trans {...LicenseeTrans} />
    case Brand:
      return <Trans {...BrandTrans} />
  }
}

export const adminOrNextRoleType = (roleType) => {
  if (roleType == Admin) {
    return Admin
  }

  let list = [Reseller, Licensee, Brand]
  let index = list.findIndex((value) => value === roleType)
  return list[index + 1]
}

export const isNextRoleType = (userRoleType, roleType) => {
  let list = [Admin, Reseller, Licensee, Brand]
  let userRoleIndex = list.findIndex((item) => item === userRoleType)
  let index = list.findIndex((item) => item === roleType)
  return userRoleIndex >= 0 && userRoleIndex + 1 === index
}

export const useRoleTypeOptions = () => {
  let userRoleType = useUserRoleType()
  let list = [Admin, Reseller, Licensee, Brand]
  const TypeOption = {
    Admin: {
      label: useTrans(AdminTrans),
      value: Admin
    },
    Reseller: {
      label: useTrans(ResellerTrans),
      value: Reseller
    },
    Licensee: {
      label: useTrans(LicenseeTrans),
      value: Licensee
    },
    Brand: {
      label: useTrans(BrandTrans),
      value: Brand
    }
  }
  let userRoleIndex = list.findIndex((value) => value === userRoleType)
  if (userRoleIndex < 0) {
    return []
  }

  let opitons = list
    .slice(userRoleIndex)
    .map((type) => Reflect.get(TypeOption, type))
  return opitons
}

export const RoleTypeSelect = ({
  options = useRoleTypeOptions(),
  ...props
}) => {
  return <Select options={options} {...props} />
}
