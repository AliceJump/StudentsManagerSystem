import docx

doc = docx.Document(r'd:\items\project\CS_project\StudentsManagerSystem\docs\专业综合实验报告模板.docx')
for i, p in enumerate(doc.paragraphs):
    txt = p.text[:100].replace('\n', '\\n')
    style = p.style.name
    runs_info = [(r.text[:30], r.font.name, r.font.size) for r in p.runs]
    print(f'[{i:3d}] style={style:15s} runs={len(p.runs)} | "{txt}"')
    if p.runs:
        for j, r in enumerate(p.runs):
            rt = r.text[:50].replace('\n','\\n')
            print(f'       run[{j}]: font={r.font.name}, size={r.font.size}, text="{rt}"')
