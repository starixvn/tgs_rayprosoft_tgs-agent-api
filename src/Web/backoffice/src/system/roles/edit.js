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
    FieldStack,
    FieldWrapper,
    Table,
    Set
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
    adminOrNextRoleType,
    isNextRoleType,
    findParentRoleType,
    isSameOrAfterRoleType
} from "./type"
import { MerchantIdSelect } from "../../merchant/MerchantSelect"
import { FormDialog } from "../../form/FormDialog"
import { useForm, useFormContext } from "react-hook-form"
import equals from "fast-deep-equal"
import Input from "../../input/Input"
import { Code as RoleMenuCode } from "./menu"
import { useResellerLabel } from "../../merchant/resellers"
import ResellerIdSelect from "../../merchant/resellers/ResellerIdSelect"
import { useLicenseeLabel } from "../../merchant/licensees"
import LicenseeIdSelect from "../../merchant/licensees/LicenseeIdSelect"
import { useBrandLabel } from "../../merchant/brands"
import BrandIdSelect from "../../merchant/brands/BrandIdSelect"
import Checkbox from "../../checkbox/Checkbox"
import Select from "../../select/Select"

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
    let { register, watch, setValue, getValues } = formMethods

    let userRoleType = useUserRoleType()
    let roleType = watch("roleType")
    let sameRoleType = userRoleType === roleType

    let userRoleId = useUserRoleId()
    let templateId = watch("templateId")

    let [formData, setMenusData] = useState({
        roleType: userRoleType,
        roleId: userRoleId
    })

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
        let roleId = sameRoleType ? userRoleId : templateId

        let newState = {
            roleType,
            roleId
        }

        if (
            newState.roleType &&
            newState.roleId &&
            !equals(newState, formData)
        ) {
            setMenusData(newState)
        }
    }, [roleType, templateId])

    let isRoot = watch("isRoot")

    useEffect(() => {
        setMenus(
            cloneDeep(sameRoleType || !isRoot ? dropRoleModule(menus) : menus)
        )
    }, [JSON.stringify(menus), userRoleType, roleType, isRoot])

    let title = useTrans({
        zh: "新增权限组",
        en: "Add Role"
    })

    const clearResult = () => {
        setSubmit(undefined)
    }

    //Note.
    // same role type -> user role id
    // next role type + isRoot -> user role id
    // others will not use user role id, template id will be required
    let useUserRoleAsTemplate =
        sameRoleType || (isNextRoleType(userRoleType, roleType) && isRoot)
    let roleTemplateRequired = !useUserRoleAsTemplate

    return (
        <FormDialog
            formMethods={formMethods}
            onSubmit={(data) => {
                let templateId = useUserRoleAsTemplate
                    ? userRoleId
                    : getValues("templateId")

                let formData = {
                    templateId,
                    roleType,
                    resellerId,
                    licenseeId,
                    brandId,
                    menus: pickDisplayMenus(menusState),
                    ...data
                }

                if (
                    isSameOrAfterRoleType(RoleTypes.Reseller, userRoleType) &&
                    !formData.resellerId
                ) {
                    formData.resellerId = userResellerId
                }
                if (
                    isSameOrAfterRoleType(RoleTypes.Licensee, userRoleType) &&
                    !formData.licenseeId
                ) {
                    formData.licenseeId = userLicenseeId
                }
                if (
                    isSameOrAfterRoleType(RoleTypes.Brand, userRoleType) &&
                    !formData.brandId
                ) {
                    formData.brandId = userBrandId
                }

                setSubmit(formData)
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
                                />
                                {roleType != RoleTypes.Admin && (
                                    <MerchantIdSelect
                                        start={adminOrNextRoleType(
                                            userRoleType
                                        )}
                                        end={roleType}
                                        resellerProps={{
                                            defaultValue: userResellerId
                                        }}
                                        licenseeProps={{
                                            defaultValue: userLicenseeId
                                        }}
                                        brandProps={{
                                            defaultValue: userBrandId
                                        }}
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
                                    <Checkbox name="isRoot" />
                                </Box>
                            </>
                        )}

                        {roleTemplateRequired && (
                            <>
                                <Label name="roleType" required>
                                    <Trans zh="权限组样板" en="Role Template" />
                                </Label>
                                <Box>
                                    <RoleTemplateSelect
                                        defaultValue={userRoleId}
                                        roleType={roleType}
                                        resellerId={resellerId}
                                        licenseeId={licenseeId}
                                        brandId={brandId}
                                    />
                                </Box>
                            </>
                        )}
                    </FieldValues>
                )}
                <DialogBody name="" menus={menusState} setMenus={setMenus} />
            </>
        </FormDialog>
    )
}

const dropRoleModule = (menus = []) => {
    return menus.map((moduleMenu) => {
        return {
            ...moduleMenu,
            subMenus: moduleMenu.subMenus.filter((menu) => {
                return menu.code.toLowerCase() != RoleMenuCode
            })
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

const useRoleListParams = () => {
    let userResellerId = useUserResellerId()
    let userLicenseeId = useUserLicenseeId()
    let userBrandId = useUserBrandId()

    let { register, watch, getValues } = useFormContext()
    let showAll = true
    let roleType = watch("roleType")
    let isRoot = watch("isRoot")
    let resellerId = watch("resellerId") ?? userResellerId
    let licenseeId = watch("licenseeId") ?? userLicenseeId
    let brandId = watch("brandId") ?? userBrandId

    if (isRoot) {
        switch (roleType) {
            case RoleTypes.Reseller: //from admin
                return {
                    showAll
                }
            case RoleTypes.Licensee:
                return {
                    isRoot: true,
                    resellerId
                }
            case RoleTypes.Brand:
                return {
                    isRoot: true,
                    resellerId,
                    licenseeId
                }
        }
    } else if (!isRoot) {
        switch (roleType) {
            case RoleTypes.Reseller: //from admin
                return {
                    showAll,
                    isRoot: true,
                    resellerId
                }
            case RoleTypes.Licensee:
                return {
                    showAll,
                    isRoot: true,
                    resellerId,
                    licenseeId
                }
            case RoleTypes.Brand:
                return {
                    showAll,
                    isRoot: true,
                    resellerId,
                    licenseeId,
                    brandId
                }
        }
    }
}

const RoleTemplateSelect = ({
    name = "templateId",
    defaultValue,
    ...props
}) => {
    let { watch, setValue, formState } = useFormContext()
    let isRoot = watch("isRoot")

    let roleType = watch("roleType")
    let resellerId = watch("resellerId")
    let licenseeId = watch("licenseeId")
    let brandId = watch("brandId")

    let templateId = watch(name)

    let targetRoleType = isRoot ? findParentRoleType(roleType) : roleType
    let params = useRoleListParams()

    let { data: roleData } = useRoleList(params)
    let placeholder = useTrans({
        zh: "选择样板",
        en: "Select Template"
    })

    let options =
        roleData?.items
            ?.filter((item) => item.roleType === targetRoleType)
            .map((role) => ({
                label: role.name,
                value: role.id
            })) ?? []

    useEffect(() => {
        if (!templateId && options.length === 1) {
            //auto select if there's only 1 option
            let option = options[0]
            setValue(name, option.value)
        }

        /* TODO: able to fix side effects? */
        /*
    let isRootTouched = formState?.touched?.isRoot
    if (!isRootTouched && !isRoot && roleData?.totalCount === 0) {
      setValue("isRoot", true)
    }
    */
    }, [options, isRoot, resellerId, licenseeId, brandId])

    return (
        <Select
            name={name}
            required
            defaultValue={defaultValue}
            placeholder={placeholder}
            options={options}
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
                <Input name="name" defaultValue={name} required />
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

export const EditButton = ({ id, record, onSubmitted, ...props }) => {
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
                <EditRoleDialog
                    id={id}
                    record={record}
                    onSubmitted={onSubmitted}
                    dialog={dialog}
                />
            )}
        </>
    )
}

const EditRoleDialog = ({ id, record, dialog, onSubmitted }) => {
    let { role, isLoading, isValidating, error, mutate, deleteCache } = useRole(
        id
    )
    let userRoleType = useUserRoleType()
    let [menus, setMenus] = useState()

    useEffect(() => {
        if (role && !role.isRoot) {
            role.menus = dropRoleModule(role.menus)
        }
        setMenus(role?.menus)
    }, [userRoleType, role])

    if (error) {
        return <AlertDialog {...error} onClickClose={dialog.hide} />
    }

    if (isLoading || !menus) {
        return <LoadingBackdrop visible={true} />
    }

    return (
        <EditDialog
            id={id}
            role={role}
            name={role?.name}
            menus={menus}
            record={record}
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
    record,
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
        <FormDialog
            onSubmit={(data) => {
                setSubmit({
                    menus: pickDisplayMenus(menusState),
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

                        {record?.resellerId && (
                            <>
                                <Label>{useResellerLabel()}</Label>
                                <ResellerIdSelect
                                    value={record?.resellerId}
                                    disabled
                                />
                            </>
                        )}

                        {record?.licenseeId && (
                            <>
                                <Label>{useLicenseeLabel()}</Label>
                                <LicenseeIdSelect
                                    value={record?.licenseeId}
                                    disabled
                                />
                            </>
                        )}

                        {record?.brandId && (
                            <>
                                <Label>{useBrandLabel()}</Label>
                                <BrandIdSelect
                                    value={record?.brandId}
                                    disabled
                                />
                            </>
                        )}
                    </FieldValues>
                )}
                <DialogBody
                    name={name}
                    menus={menusState}
                    setMenus={setMenus}
                />
            </>
        </FormDialog>
    )
}

const MenuTable = ({ menus = [], setChecked } = {}) => {
    return (
        <Table hasDividers>
            {menus.map((menu, index) => {
                return (
                    <MenuTableContent
                        key={index}
                        menu={menu}
                        setChecked={setChecked}
                    />
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
                    let {
                        code: subMenuCode,
                        subMenus: subMenuItems = []
                    } = menu
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
