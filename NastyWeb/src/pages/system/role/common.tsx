
export const roleTypes = [
    { label: "系统", value: 1 },
    { label: "部门", value: 2 },
    { label: "普通", value: 3 },
]

export const roleType = {
    System: 1,
    Department: 2,
    Normal: 3,
}

export const getRoleTypeText = (v: any) => {
    let type = roleTypes.find((t) => t.value == v);
    return type?.label || "-";
}

export const isNormalRole = (v: any) => {
    return v == 3;
}