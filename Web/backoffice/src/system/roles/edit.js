import React, { useState, useEffect, useMemo, useRef, Children } from "react"
import { css, Global } from "@emotion/react"
import {
  Spinner,
  Stack,
  Flex,
  Box,
  Card,
  Button,
  Text,
  Input,
  Checkbox,
  FieldStack,
  FieldWrapper,
  SelectField,
  Table,
  Select,
  Group,
  Set,
  useClassName,
  InputField as _InputField
} from "bumbag"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import AlertDialog from "../../dialog/Alert"
import { Create as CreateMenu, Update as UpdateMenu } from "./menu"
import {
  useIsDisplayMenuByRoute,
  useUserRoleType,
  useUserRoleId,
  useUserResellerId,
  useUserLicenseeId,
  useUserBrandId
} from "../../user/context"
import { useLabel as useMenuLabel, find as findMenu } from "../../menu"
import {
  useMenus,
  useAddRole,
  useEditRole,
  useRole,
  useRoleList
} from "./client"
import Disclosure from "../../dialog/Disclosure"
import { Trans, useTrans } from "../../i18n"
import { FieldValues } from "../../layout/Grid"
import Label from "../../form/Label"
import { useModalState } from "../../dialog/Modal"
import cloneDeep from "clone-deep"
import {
  RoleTypeSelect,
  RoleTypes,
  useRoleTypeOptions,
  RoleTypeLabel,
  adminOrNextRoleType
} from "./type"
import { useBrandList } from "../../merchant/brands/client"
import { Reseller } from "../../merchant/resellers"
import { MerchantIdSelect } from "../../merchant/MerchantSelect"
import { HookFormDialog } from "../../form/HookFormDialog"
import { useForm, useFormContext } from "react-hook-form"
import equals from "fast-deep-equal"
import { HookFormInputField } from "../../input/InputField"
import { Code as RoleMenuCode } from "./menu"

export const AddButton = ({ onSubmitted, ...props }) => {
  let dialog = useModalState()
  let isDisplay = useIsDisplayMenuByRoute([CreateMenu])
  if (!isDisplay) {
    return null
  }

  return (
    <>
      <Disclosure {...dialog}>
        <Trans zh="新增" en="Add" />
      </Disclosure>
      {dialog.visible && (
        <AddDialog onSubmitted={onSubmitted} dialog={dialog} />
      )}
    </>
  )
}

const AddDialog = ({ dialog, onSubmitted }) => {
  let formMethods = useForm()
  let { register, watch, setValue } = formMethods

  let userRoleType = useUserRoleType()
  let roleType = watch("roleType")

  let userRoleId = useUserRoleId()
  let roleId = watch("roleId")

  let [formData, setMenusData] = useState({
    roleType: userRoleType,
    roleId: userRoleId
  })

  useEffect(() => {
    let newState = {
      roleType,
      roleId
    }
    if (newState.roleType && roleType.roleId && !equals(newState, formData)) {
      setMenusData(newState)
    }
  }, [roleType, roleId])

  let {
    menus,
    isLoading: isLoadingMenus,
    error: roleMenusError,
    mutate
  } = useMenus(formData)

  let userResellerId = useUserResellerId()
  let resellerId = watch("resellerId")

  let userLicenseeId = useUserLicenseeId()
  let licenseeId = watch("licenseeId")

  let userBrandId = useUserBrandId()
  let brandId = watch("brandId")

  let roleTypeOptions = useRoleTypeOptions()
  let [menusState, setMenus] = useState()
  let [submitData, setSubmit] = useState()
  let { result, error, isLoading } = useAddRole(submitData)

  useEffect(() => {
    let isSame = userRoleType == roleType
    setMenus(cloneDeep(isSame ? dropRoleModule(menus) : menus))
  }, [JSON.stringify(menus), userRoleType, roleType])

  let title = useTrans({
    zh: "新增权限组",
    en: "Add Role"
  })

  const clearResult = () => {
    setSubmit(undefined)
  }

  let showRoleSelect =
    userRoleType != RoleTypes.Admin &&
    ((roleType === RoleTypes.Licensee && resellerId) ||
      (roleType === RoleTypes.Brand && licenseeId))

  return (
    <HookFormDialog
      formMethods={formMethods}
      onSubmit={(data) => {
        setSubmit({
          roleType,
          resellerId,
          licenseeId,
          brandId,
          menus: pickDisplayMenus(menusState),
          ...data
        })
      }}
      isLoading={isLoading}
      result={result}
      error={error}
      onCloseErrorAlert={clearResult}
      onSubmitted={onSubmitted}
      title={title}
      dialog={dialog}
      placement="top"
      maxWidth="none"
      width="40vw"
      css={css`
        & .bb-DialogContent {
          position: relative;
        }
      `}
    >
      <>
        {roleTypeOptions.length > 1 && (
          <FieldValues margin="major-1">
            <Label name="roleType">
              <Trans zh="权限组类型" en="Role Type" />
            </Label>
            <Box>
              <Set orientation="horizontal">
                <RoleTypeSelect
                  name="roleType"
                  required
                  defaultValue={userRoleType}
                  selectRef={register({ required: true })}
                />
                {roleType != RoleTypes.Admin && (
                  <MerchantIdSelect
                    start={adminOrNextRoleType(userRoleType)}
                    end={roleType}
                    resellerId={userResellerId}
                    licenseeId={userLicenseeId}
                    brandId={userBrandId}
                  />
                )}
              </Set>
            </Box>

            {userRoleType != roleType && (
              <>
                <Label name="root">
                  <Trans zh="根管理权限" en="Root Admin" />
                </Label>
                <Box alignY="center">
                  <Checkbox name="isRoot" checkboxRef={register} />
                </Box>
              </>
            )}

            {showRoleSelect && (
              <>
                <Label name="roleType">
                  <Trans zh="权限组" en="Role" />
                </Label>
                <Box>
                  <RoleIdSelect
                    name="roleId"
                    defaultValue={userRoleId}
                    roleType={roleType}
                    resellerId={resellerId}
                    licenseeId={licenseeId}
                  />
                </Box>
              </>
            )}
          </FieldValues>
        )}
        <DialogBody name="" menus={menusState} setMenus={setMenus} />
      </>
    </HookFormDialog>
  )
}

const dropRoleModule = (menus = []) => {
  return menus.map((menu) => {
    return {
      ...menu,
      subMenus: menu.subMenus.filter(
        (menu) => menu.code.toLowerCase() != RoleMenuCode
      )
    }
  })
}

const pickDisplayMenus = (menus = []) => {
  return menus
    .filter((menu) => menu.isDisplay)
    .map((menu) => {
      menu.subMenus = pickDisplayMenus(menu.subMenus)
      return menu
    })
}

const RoleIdSelect = ({
  name = "roleId",
  defaultValue,
  roleType,
  resellerId,
  licenseeId,
  ...props
}) => {
  let { register } = useFormContext()
  let { data: roleData } = useRoleList(
    roleType === RoleTypes.Licensee && resellerId
      ? { showAll: true, resellerId }
      : roleType === RoleTypes.Brand && licenseeId
      ? { showAll: true, licenseeId }
      : undefined
  )

  let placeholder = useTrans({
    zh: "预设",
    en: "Default"
  })
  if (!roleData?.items) {
    return null
  }

  return (
    <Select
      name={name}
      required
      defaultValue={defaultValue}
      selectRef={register({ required: true })}
      placeholder={placeholder}
      options={roleData.items.map((role) => ({
        label: role.name,
        value: role.id
      }))}
    />
  )
}

const DialogBody = ({ name = "", menus, setMenus }) => {
  return (
    <FieldValues margin="major-1">
      <Label name="name" required>
        <Trans zh="权限组名称" en="Name" />
      </Label>
      <Box>
        <HookFormInputField name="name" defaultValue={name} required />
      </Box>

      <Label>
        <Trans zh="权限管理" en="Role Settings" />
      </Label>
      <MenusBody menus={menus} setMenus={setMenus} />
    </FieldValues>
  )
}

const MenusBody = ({ menus = [], setMenus }) => {
  const setChecked = (path = [], checked) => {
    let item = findMenu(path, menus)
    item.isDisplay = checked

    let parentPath = path.slice(0, path.length - 1)
    setSubMenusDisplay(item.isDisplay, item)
    syncMenuDisplay(parentPath, menus)
    if (checked) {
      autoDisplayParentMenu(parentPath, menus)
    }

    setMenus([...menus])
  }

  return <MenuTable menus={menus} setChecked={setChecked} />
}

const setSubMenusDisplay = (display, menu = {}) => {
  let subMenus = menu?.subMenus ?? []
  subMenus.forEach((menu) => {
    menu.isDisplay = display
    setSubMenusDisplay(display, menu)
  })
}

const syncMenuDisplay = (path = [], menus) => {
  if (!path || path.length === 0) {
    return
  }

  let menu = findMenu(path, menus) ?? {}
  let subMenus = menu.subMenus ?? []
  let { isSameDisplay, isDisplay } = subMenus.reduce(
    (context, menu, index) => {
      if (index === 0) {
        context.isDisplay = menu.isDisplay
      }
      if (context.isDisplay != menu.isDisplay) {
        context.isSameDisplay = false
      }
      return context
    },
    {
      isSameDisplay: true,
      isDisplay: false
    }
  )

  if (isSameDisplay) {
    menu.isDisplay = isDisplay
    if (path.length > 1) {
      syncMenuDisplay(path.slice(0, path.length - 1), menus)
    }
  }
}

const autoDisplayParentMenu = (path = [], menus) => {
  if (!path || path.length === 0) {
    return
  }

  let menu = findMenu(path, menus) ?? {}
  if (menu) {
    menu.isDisplay = true
  }

  if (path.length > 1) {
    autoDisplayParentMenu(path.slice(0, path.length - 1), menus)
  }
}

export const EditButton = ({ id, onSubmitted, ...props }) => {
  let dialog = useModalState()

  let isDisplay = useIsDisplayMenuByRoute([UpdateMenu])
  if (!isDisplay) {
    return null
  }

  return (
    <>
      <Disclosure {...dialog}>
        <Trans zh="修改" en="Edit" />
      </Disclosure>
      {dialog.visible && (
        <EditRoleDialog id={id} onSubmitted={onSubmitted} dialog={dialog} />
      )}
    </>
  )
}

const EditRoleDialog = ({ id, dialog, onSubmitted }) => {
  let userRoleType = useUserRoleType()
  let userRoleId = useUserRoleId()

  let { role, isLoading, isValidating, error, mutate, deleteCache } = useRole(
    id
  )

  if (isLoading) {
    return <LoadingBackdrop visible={true} />
  }

  if (error) {
    return <AlertDialog {...error} onClickClose={dialog.hide} />
  }

  return (
    <EditDialog
      id={id}
      role={role}
      name={role?.name}
      menus={role?.menus}
      dialog={dialog}
      onSubmitted={(data) => {
        mutate(false, undefined)
        onSubmitted && onSubmitted()
      }}
    />
  )
}

const EditDialog = ({
  id,
  role,
  name = "",
  menus = [],
  dialog,
  onSubmitted
}) => {
  let [menusState, setMenus] = useState(cloneDeep(menus))
  let [submitData, setSubmit] = useState()
  let { result, error, isLoading } = useEditRole(id, submitData)

  useEffect(() => {
    setMenus(cloneDeep(menus))
  }, [menus])

  let title = useTrans({
    zh: "修改权限组",
    en: "Edit Role"
  })

  const clearResult = () => {
    setSubmit(null)
  }

  let roleTypeOptions = useRoleTypeOptions()

  return (
    <HookFormDialog
      onSubmit={(data) => {
        setSubmit({
          menus: menusState,
          ...data
        })
      }}
      isLoading={isLoading}
      result={result}
      error={error}
      onCloseErrorAlert={clearResult}
      onSubmitted={() => {
        onSubmitted && onSubmitted(submitData)
      }}
      title={title}
      dialog={dialog}
      placement="top"
      maxWidth="none"
      width="40vw"
      css={css`
        & .bb-DialogContent {
          position: relative;
        }
      `}
    >
      <>
        {roleTypeOptions.length > 1 && (
          <FieldValues margin="major-1">
            <Label name="roleType">
              <Trans zh="权限组类型" en="Role Type" />
            </Label>
            <Label fontWeight="normal">
              <RoleTypeLabel value={role.roleType} />
            </Label>
          </FieldValues>
        )}
        <DialogBody name={name} menus={menusState} setMenus={setMenus} />
      </>
    </HookFormDialog>
  )
}

const MenuTable = ({ menus = [], setChecked } = {}) => {
  return (
    <Table hasDividers>
      {menus.map((menu, index) => {
        return (
          <MenuTableContent key={index} menu={menu} setChecked={setChecked} />
        )
      })}
    </Table>
  )
}

const MenuTableContent = ({ menu = {}, setChecked }) => {
  let { code: moduleCode, subMenus = [] } = menu
  let label = useMenuLabel({ codes: [moduleCode] })
  if (!label) {
    return null
  }

  return (
    <>
      <Table.Head>
        <Table.Row>
          <Table.HeadCell colSpan="2">
            <MenuCheckbox menu={menu} setChecked={setChecked} />
          </Table.HeadCell>
        </Table.Row>
      </Table.Head>
      <Table.Body>
        {subMenus.map((menu) => {
          let { code: subMenuCode, subMenus: subMenuItems = [] } = menu
          return (
            <Table.Row key={subMenuCode}>
              <Table.Cell>
                <MenuCheckbox
                  path={[moduleCode]}
                  menu={menu}
                  setChecked={setChecked}
                />
              </Table.Cell>
              <Table.Cell>
                <span>
                  {subMenuItems.map((subMenuItem, index) => {
                    return (
                      <MenuCheckbox
                        key={index}
                        path={[moduleCode, subMenuCode]}
                        menu={subMenuItem}
                        setChecked={setChecked}
                      />
                    )
                  })}
                </span>
              </Table.Cell>
            </Table.Row>
          )
        })}
      </Table.Body>
    </>
  )
}

const MenuCheckbox = ({ path = [], menu = {}, setChecked }) => {
  let { code } = menu
  let menuPath = [...path, code]
  let label = useMenuLabel({ codes: menuPath })
  if (!label) {
    return null
  }

  return (
    <Checkbox
      label={label}
      checked={menu.isDisplay}
      onChange={(event) => {
        let isChecked = event.target.checked
        setChecked(menuPath, isChecked)
      }}
      whiteSpace="nowrap"
      display="inline-flex"
      marginRight="major-1"
    ></Checkbox>
  )
}
