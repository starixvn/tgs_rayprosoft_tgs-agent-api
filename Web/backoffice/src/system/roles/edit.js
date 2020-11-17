import React, { useState, useEffect, useMemo, useRef, Children } from "react"
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
  Input,
  Checkbox,
  FieldStack,
  FieldWrapper,
  SelectField,
  Table,
  useClassName
} from "bumbag"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import AlertDialog from "../../dialog/Alert"
import { Create as CreateMenu, Update as UpdateMenu } from "./menu"
import { useIsDisplayMenuByRoute } from "../../user/context"
import { useLabel as useMenuLabel, find as findMenu } from "../../menu"
import { useMenus, useAddRole, useEditRole, useRole } from "./client"
import Disclosure from "../../dialog/Disclosure"
import { Trans, useTrans } from "../../i18n"
import { FieldValues } from "../../layout/Grid"
import Label from "../../form/Label"
import InputField from "../../input/Field"
import { Modal, useModalState } from "../../dialog/Modal"
import FormDialog from "../../form/Dialog"
import cloneDeep from "clone-deep"

export const AddButton = ({ onSubmitted, ...props }) => {
  let dialog = useModalState()
  let {
    menus,
    isLoading: isLoadingMenus,
    error: fetchMenusError,
    mutate
  } = useMenus(dialog.visible)

  useEffect(() => {
    if (!dialog.visible) {
      mutate()
    }
  }, [dialog.visible])

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
        <AddDialog
          menus={menus}
          onSubmitted={onSubmitted}
          dialog={dialog}
          visible={dialog.visible && !isLoadingMenus}
        />
      )}
      {dialog.visible && isLoadingMenus && <LoadingBackdrop visible={true} />}
      {dialog.visible && fetchMenusError && (
        <AlertDialog {...fetchMenusError} onClickClose={dialog.hide} />
      )}
    </>
  )
}

const AddDialog = ({ menus = [], dialog, onSubmitted }) => {
  let [menusState, setMenus] = useState()
  let [submitData, setSubmit] = useState()
  let { result, error, isLoading } = useAddRole(submitData)

  useEffect(() => {
    setMenus(cloneDeep(menus))
  }, [JSON.stringify(menus)])

  let title = useTrans({
    zh: "新增权限组",
    en: "Add Role"
  })

  const clearResult = () => {
    setSubmit(null)
  }

  return (
    <>
      <FormDialog
        initialValues={{
          name: ""
        }}
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
        onSubmitted={onSubmitted}
        title={title}
        dialog={dialog}
        placement="top"
        maxWidth="none"
        width="40vw"
      >
        <DialogBody menus={menusState} setMenus={setMenus} dialog={dialog} />
      </FormDialog>
    </>
  )
}

const DialogBody = ({ menus, setMenus, dialog }) => {
  return (
    <FieldValues margin="major-1" height="100%">
      <Label name="name" required>
        <Trans zh="权限组名称" en="Name" />
      </Label>
      <Box>
        <InputField name="name" />
      </Box>

      <Label>
        <Trans zh="权限管理" en="Role" />
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
      isDisplay
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
        <AsyncRoleDialog id={id} onSubmitted={onSubmitted} dialog={dialog} />
      )}
    </>
  )
}

const AsyncRoleDialog = ({ id, dialog, onSubmitted }) => {
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
      name={role.name}
      menus={role.menus}
      dialog={dialog}
      onSubmitted={(data) => {
        mutate({
          ...role,
          ...data
        })
        onSubmitted && onSubmitted()
      }}
    />
  )
}

const EditDialog = ({ id, name, menus, dialog, onSubmitted }) => {
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

  return (
    <FormDialog
      initialValues={{
        name
      }}
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
    >
      {({ resetForm, ...formProps }) => {
        useEffect(() => {
          resetForm({
            values: {
              name
            }
          })
        }, [name])

        return (
          <DialogBody menus={menusState} setMenus={setMenus} dialog={dialog} />
        )
      }}
    </FormDialog>
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
